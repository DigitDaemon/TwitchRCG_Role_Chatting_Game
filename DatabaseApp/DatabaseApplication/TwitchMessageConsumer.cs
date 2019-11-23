using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DatabaseApplicationML.Model;
using System.IO;
using System.Data.Odbc;
using System.Data;

namespace DatabaseApplication
{
    class TwitchMessageConsumer
    {

        //vars: Kafka Consumer Config
        //
        //server - the kafka server to connect to
        //topic - the topic to subscribe to
        //config - an object containing configuration information for the Kafka connection
        //cancelToken - a token used to cancel a the Kafka consumer looing for new messages
        //source - the source of <cancelToken>
        static string server = "Localhost:9092";
        private bool Active;
        string topic = "twitch_message";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //var: commandeMessageQueue
        //The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
        //this is shared by the <CommandProducer> the <TwitchCommandConsumer> and the <TwitchMessageConsumer>
        ConcurrentQueue<string> commandMessageQueue;

        //objs: ML Objects
        //
        //mOutput - the output of the machine learning algorthm
        //mInput - the input into the machine learning algorithm
        ModelOutput mOutput;
        ModelInput mInput;

        //strings: ODBC configuration
        //
        //path - the path to the folder containing sensitive information for this project on my computer
        //dbConnString - the text contained inside the Connection.txt file with information for connecting to the database
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!

        //objs: ODBS objects
        //
        //data - the data returned from a query
        //dbAdapter - the object that handles retrival of data from the database
        //dbCommand - the command builder for <dbAdapter>
        //connDB - the connection to the database
        DataSet data;
        OdbcDataAdapter dbAdapter;
        OdbcCommandBuilder dbCommand;
        OdbcConnection connDB;

        /**
         * The constructor for <TwitchMessageConsumer>
         * 
         * Parameters:
         * 
         * commandMessageQueue - The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
         */
        public TwitchMessageConsumer(ref ConcurrentQueue<string> commandMessageQueue)
        {
            config = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = Guid.NewGuid().ToString(),
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true,

            };
            this.commandMessageQueue = commandMessageQueue;
            source = new CancellationTokenSource();
            canceltoken = source.Token;

            connDB = new OdbcConnection(dbConnString);
            try
            {
                connDB.Open();
            }
            catch(OdbcException e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }
            data = new DataSet();
            dbAdapter = new OdbcDataAdapter();
            dbCommand = new OdbcCommandBuilder(dbAdapter);
        }

        public void TMCThread()
        {
            Console.WriteLine("TwitchMessageThread Start");
            Active = true;

            //objs: Kafka Consumer client
            //
            //consumer - the Kafka consumer client
            //topicp - the topic partition from <consumer> to consume from
            var consumer = new ConsumerBuilder<string, string>(config).Build();
            var topicp = new TopicPartition(topic, 0);

            consumer.Assign(topicp);

            while (Active)
            {
                
                try
                {
                    //var: consumerresult
                    //The result of checking for the next new message on the Kafka server
                    var consumeresult = consumer.Consume(canceltoken);

                    if (!consumeresult.IsPartitionEOF)
                    {
                        //strings: Kafka Message
                        //
                        //input - the raw message from Kafka
                        //channel - the channel that the message was received from
                        //uname - the username of the user that sent the message
                        //message - the actual message portion
                        var input = consumeresult.Value;
                        var channel = input.Substring(0, input.IndexOf(" "));
                        var uname = input.Substring(input.IndexOf(" ")).Trim();
                        var message = uname.Substring(uname.IndexOf(" ")).Trim();
                        uname = uname.Substring(0, uname.IndexOf(" ")).Trim();
                        uname = uname.TrimStart(new char[] { '\0', ':'});

                        try
                        {
                            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
                            dbAdapter.Fill(data);

                            if (data.Tables[0].Rows.Count != 0)
                            {
                                mInput = new ModelInput();
                                mInput.Message = message;
                                mOutput = ConsumeModel.Predict(mInput);
                                //string: result
                                //the result of the message being passed through the machine learning algorithm
                                string result = mOutput.Prediction;
                                if (data.Tables[0].Rows[0]["growing"].ToString().Equals("1"))
                                {
                                    string attribute = "";
                                    int value = 0;

                                    if (result.Equals("Temper"))
                                    {
                                        attribute = "temper";
                                        value = int.Parse(data.Tables[0].Rows[0]["temper"].ToString()) + 1;

                                    }
                                    else if (result.Equals("Cheer"))
                                    {
                                        attribute = "cheer";
                                        value = int.Parse(data.Tables[0].Rows[0]["cheer"].ToString()) + 1;
                                    }
                                    else if (result.Equals("Curiousity"))
                                    {
                                        attribute = "curiosity";
                                        value = int.Parse(data.Tables[0].Rows[0]["curiosity"].ToString()) + 1;
                                    }
                                    else if (result.Equals("Charisma"))
                                    {
                                        attribute = "charisma";
                                        value = int.Parse(data.Tables[0].Rows[0]["charisma"].ToString()) + 1;
                                    }
                                    else if (result.Equals("Empathy"))
                                    {
                                        attribute = "empathy";
                                        value = int.Parse(data.Tables[0].Rows[0]["empathy"].ToString()) + 1;
                                    }

                                    //obj: comm
                                    //the new query to be exicuted on the database
                                    OdbcCommand comm = new OdbcCommand("UPDATE users SET " + attribute + "=" + value + " WHERE twitch_name='" + uname + "';", connDB);
                                    comm.ExecuteNonQueryAsync().Wait();

                                    if (int.Parse(data.Tables[0].Rows[0]["temper"].ToString()) + int.Parse(data.Tables[0].Rows[0]["cheer"].ToString()) + int.Parse(data.Tables[0].Rows[0]["curiosity"].ToString()) + int.Parse(data.Tables[0].Rows[0]["charisma"].ToString()) + int.Parse(data.Tables[0].Rows[0]["empathy"].ToString()) >= 50)
                                    {
                                        comm = new OdbcCommand("UPDATE users SET growing=false WHERE twitch_name='" + uname + "';", connDB);
                                        comm.ExecuteNonQueryAsync().Wait();
                                    }
                                }

                            }
                            else
                            {
                                commandMessageQueue.Enqueue("SCPBlacklist " + uname);
                            }
                            data.Reset();
                        }
                        catch (OdbcException e)
                        {
                            
                        }
                    }
                    else
                    {
                        Thread.Yield();
                    }
                }
                catch (System.OperationCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        /**
         * This ends the loop in <TCCThread> and cancels any current attempt to fetch a new message
         * allowing the thread to end.
         */
        public void Kill()
        {
            connDB.Close();
            Active = false;
            source.Cancel();
        }
    }
}
