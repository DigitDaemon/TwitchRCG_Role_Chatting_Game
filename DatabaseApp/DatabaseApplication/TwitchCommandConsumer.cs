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
    class TwitchCommandConsumer
    {
        static string server = "Localhost:9092";
        private bool Active;
        string topic = "twitch_command";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;
        ConcurrentQueue<string> commandMessageQueue;
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        static private string dbConnString = File.ReadAllText(Path.Combine(path, "Connection.txt"));//do not push this!!!
        DataSet data;
        OdbcDataAdapter dbAdapter;
        OdbcCommandBuilder dbCommand;
        OdbcConnection connDB;

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

        public void TCCThread()
        {
            Console.WriteLine("TwitchCommandThread Start");
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
                            dbAdapter.SelectCommand = new OdbcCommand("SELECT * FROM users WHERE twitch_name='" + uname + "';", connDB);
                            dbAdapter.Fill(data);

                            if(data.Tables[0].Rows.Count == 0 || data.Tables[0].Rows[0]["discord_name"].ToString().Equals(""))
                            {
                                if (data.Tables[0].Rows.Count == 0)
                                {
                                    commandMessageQueue.Enqueue("SCPUnblacklist " + uname);
                                    
                                    if (parameter.Equals(""))
                                    {
                                        OdbcCommand comm = new OdbcCommand("INSERT INTO users (twitch_name) VALUES ('"+uname+"' )", connDB);
                                        comm.ExecuteNonQueryAsync().Wait();
                                    } else
                                    {
                                        OdbcCommand comm = new OdbcCommand("INSERT INTO users (twitch_name, discord_name) VALUES ( '"+uname+ "','"+parameter+"')", connDB);
                                        comm.ExecuteNonQueryAsync().Wait();
                                    }
                                } else if (!parameter.Equals(""))
                                {
                                    data.Tables[0].Rows[0]["discord_name"] = parameter;
                                    dbAdapter.Update(data);
                                }
                            }
                            try
                            {
                                commandMessageQueue.Enqueue("SCPUnblacklist " + uname);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
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

        public void Kill()
        {
            Active = false;
            source.Cancel();
        }
    }
}
