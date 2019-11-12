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
        static string server = "Localhost:9092";
        private bool Active;
        string topic = "twitch_message";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;
        ConcurrentQueue<string> commandMessageQueue;
        ModelOutput mOutput;
        ModelInput mInput;
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!
        DataSet data;
        OdbcDataAdapter dbAdapter;
        OdbcCommandBuilder dbCommand;
        OdbcConnection connDB;

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
            var consumer = new ConsumerBuilder<string, string>(config).Build();
            var topicp = new TopicPartition(topic, 0);
            consumer.Assign(topicp);

            while (Active)
            {
                
                try
                {
                    var consumeresult = consumer.Consume(canceltoken);

                    if (!consumeresult.IsPartitionEOF)
                    {
                        var input = consumeresult.Value;
                        var channel = input.Substring(0, input.IndexOf(" "));
                        var uname = input.Substring(input.IndexOf(" ")).Trim();
                        var message = uname.Substring(uname.IndexOf(" ")).Trim();
                        uname = uname.Substring(0, uname.IndexOf(" ")).Trim();
                        uname = uname.TrimStart(new char[] { '\0', ':'});

                        //Console.WriteLine(channel);
                        //Console.WriteLine(uname);
                        //Console.WriteLine(message);
                        try
                        {
                            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
                            dbAdapter.Fill(data);

                            if (data.Tables[0].Rows.Count != 0)
                            {
                                mInput = new ModelInput();
                                mInput.Message = message;
                                mOutput = ConsumeModel.Predict(mInput);
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

       

        public void Kill()
        {
            Active = false;
            source.Cancel();
        }
    }
}
