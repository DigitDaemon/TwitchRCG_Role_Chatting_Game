using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatabaseApplication
{
    class CommandConsumer
    {
        static string server = "Localhost:9092";
        private bool Active;
        string topic = "COMMANDS";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;
        ThreadController controller;

        public CommandConsumer(ThreadController controller)
        {
            this.controller = controller;
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

        public void CommandThread()
        {
            Console.WriteLine("COMMANDSThread Start");
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
                        string command;
                        string parameter;
                        if (input.Contains(" "))
                        {
                            command = input.Substring(0, input.IndexOf(" ")).Trim();
                            parameter = input.Substring(input.IndexOf(" "), input.Length - command.Length).Trim();
                        }
                        else
                        {
                            command = input;
                            parameter = "";
                        }
                        Console.WriteLine("COMMAND----------> " + input);


                        if (command.Equals("DAExit") || command.Equals("System-Shutdown"))
                        {
                            Active = false;
                            source.Cancel();
                            controller.exit();
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
