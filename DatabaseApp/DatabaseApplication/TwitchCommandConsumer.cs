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
    /**
     * This class handles any relevent commands entered into twitch coming through the twitch_command topic in Kafka
     */
    class TwitchCommandConsumer
    {
        //vars: Kafka Consumer Config
        //
        //server - the kafka server to connect to
        //topic - the topic to subscribe to
        //config - an object containing configuration information for the Kafka connection
        //cancelToken - a token used to cancel a the Kafka consumer looing for new messages
        //source - the source of <cancelToken>
        static string server = "Localhost:9092";
        string topic = "twitch_command";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //bool: Active
        //controls the primary loop in <TCCThread>
        private bool Active;

        //var: commandeMessageQueue
        //The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
        //this is shared by the <CommandProducer> the <TwitchCommandConsumer> and the <TwitchMessageConsumer>
        ConcurrentQueue<string> commandMessageQueue;

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
         * The constructor for <TwitchCommandConsumer>
         * 
         * Parameters:
         * 
         * commandMessageQueue - The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
         */
        public TwitchCommandConsumer(ref ConcurrentQueue<string> commandMessageQueue)
        {
            this.commandMessageQueue = commandMessageQueue;

            config = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = Guid.NewGuid().ToString(),
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true

            };
            source = new CancellationTokenSource();
            canceltoken = source.Token;

            connDB = new OdbcConnection(dbConnString);
            try
            {
                connDB.Open();
            }
            catch (OdbcException e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }
            data = new DataSet();
            dbAdapter = new OdbcDataAdapter();
            dbCommand = new OdbcCommandBuilder(dbAdapter);
        }

        /**
         * This is the primary function of <TwitchCommandConsumer> and is to be run as a thread
         */
        public void TCCThread()
        {
            Console.WriteLine("TwitchCommandThread Start");
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
                    var consumerresult = consumer.Consume(canceltoken);

                    if (!consumerresult.IsPartitionEOF)
                    {
                        //strings: Kafka Message
                        //
                        //input - the raw message from Kafka
                        //channel - the channel that the message was received from
                        //uname - the username of the user that sent the message
                        //message - the actual message portion
                        //parameter - a part of the message intended to be passed as a paremeter 
                        var input = consumerresult.Value;
                        Console.WriteLine(input);
                        var channel = input.Substring(0, input.IndexOf(" "));
                        var uname = input.Substring(input.IndexOf(" ")).Trim();
                        uname = uname.TrimStart(new char[] { '\0' , ':' });
                        var message = uname.Substring(uname.IndexOf(" ")).Trim();
                        uname = uname.Substring(0, uname.IndexOf(" ")).Trim();
                        string parameter = "";
                        if (message.Contains(" "))
                        {
                            var message2 = message.Substring(0, message.IndexOf(" ")).Trim();
                            parameter = message.Substring(message.IndexOf(" "), message.Length - message2.Length).Trim();
                            message = message2;
                        }
                        Console.WriteLine("TwitchCommand----------> " + input); 

                        if (message.ToLower().Equals("!djoin"))
                        {
                            bool joined = false;
                            do
                            {
                                dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
                                dbAdapter.Fill(data);

                                if (data.Tables[0].Rows.Count == 0 || data.Tables[0].Rows[0]["discord_name"].ToString().Equals(""))
                                {
                                    if (data.Tables[0].Rows.Count == 0)
                                    {
                                        commandMessageQueue.Enqueue("SCPUnblacklist " + uname);

                                        if (parameter.Equals(""))
                                        {
                                            OdbcCommand comm = new OdbcCommand("INSERT INTO users (twitch_name) VALUES ('" + uname + "' )", connDB);
                                            comm.ExecuteNonQueryAsync().Wait();
                                        }
                                        else
                                        {
                                            OdbcCommand comm = new OdbcCommand("INSERT INTO users (twitch_name, discord_name) VALUES ( '" + uname + "','" + parameter + "')", connDB);
                                            comm.ExecuteNonQueryAsync().Wait();
                                        }
                                    }
                                    else if (!parameter.Equals(""))
                                    {
                                        data.Tables[0].Rows[0]["discord_name"] = parameter;
                                        dbAdapter.Update(data);
                                    }
                                }
                                try
                                {
                                    commandMessageQueue.Enqueue("SCPUnblacklist " + uname);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    Console.WriteLine(e.StackTrace);
                                }

                                data.Reset();
                                dbAdapter.Fill(data);
                                if (data.Tables[0].Rows.Count > 0)
                                {
                                    joined = true;
                                    data.Reset();
                                }

                            } while (joined == false);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (System.OperationCanceledException e)
                {

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
