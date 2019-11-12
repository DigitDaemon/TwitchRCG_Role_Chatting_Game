using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameCommandConsumer
{ 

    class GameCommandConsumer
    {
        static string server = "Localhost:9092";
        private bool Active;
        string topic = "twitch_command";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;
        ConcurrentQueue<string> gameMessageQueue;
        ConcurrentQueue<string> otherMessageQueue;
        private static List<string> GAME_START_FILTER = new List<string>(){ "!dQuest" };
        private static List<string> OTHER_MESSAGE_FILTER = new List<string>() { "placeholder" };
        public GameCommandConsumer(ref ConcurrentQueue<string> gameMessageQueue, ref ConcurrentQueue<string> otherMessageQueue)
        {
            this.gameMessageQueue = gameMessageQueue;
            this.otherMessageQueue = otherMessageQueue;

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
        }

        public void CommandConsumerThread()
        {
            Console.WriteLine("CommandConsumerThread start.");
            Active = true;

            while (Active)
            {
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
                            string command;
                            string username;
                            string channel;
                            

                            channel = input.Substring(0, input.IndexOf(" ")).Trim();
                            command = input.Substring(input.IndexOf(" "), input.Length - channel.Length).Trim();
                            username = command.Substring(0, command.IndexOf(" ")).Trim();
                            command = command.Substring(command.IndexOf(" "), command.Length - username.Length).Trim();
                            username.TrimStart(new char[] { '\0', ':' });

                            Console.WriteLine("GameCommand----------> " + input);

                            if (GAME_START_FILTER.Contains(command))
                            {
                                gameMessageQueue.Enqueue(input);
                            }
                            else if (OTHER_MESSAGE_FILTER.Contains(command))
                            {
                                otherMessageQueue.Enqueue(input);
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
        }

        public void Kill()
        {
            Active = false;
            source.Cancel();
        }
    }
}
