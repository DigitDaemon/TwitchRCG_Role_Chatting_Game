using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameApplication
{
    /**
     * This class reads in commands from the Kafka COMMANDS topic.
     */
    class KafkaCommandConsumer
    {
        //vars: Kafka Consumer Config
        //
        //server - the kafka server to connect to
        //topic - the topic to subscribe to
        //config - an object containing configuration information for the Kafka connection
        //cancelToken - a token used to cancel a the Kafka consumer looing for new messages
        //source - the source of <cancelToken>
        static string server = "Localhost:9092";
        string topic = "COMMANDS";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //vars: class configuration
        //
        //controller - the object that instantiated this object
        //Active - controls the main loop
        ThreadController controller;
        private bool Active;

        /**
         * the contructor for the <KafkaConsumer> class
         * 
         * parameters:
         * 
         * controller - the object that created this one. Needed for exicuting IPC commands
         */
        public KafkaCommandConsumer(ThreadController controller)
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

        /**
         * the primary function of the <KafkaConsumer> to be run as a thread
         */
        public void CommandThread()
        {
            Console.WriteLine("COMMANDSThread Start");
            Active = true;

            //objs: Kafka Consumer
            //
            //consumer - the Kafka consumer object
            //topicp - the partition to subscribe to
            var consumer = new ConsumerBuilder<string, string>(config).Build();
            var topicp = new TopicPartition(topic, 0);

            consumer.Assign(topicp);
            while (Active)
            {
                try
                {
                    //var: consumerresult
                    //
                    //the result of checking for a new message from the Kafka server
                    var consumeresult = consumer.Consume(canceltoken);

                    if (!consumeresult.IsPartitionEOF)
                    {
                        //vars: Kafka message
                        //
                        //command - the part of the message that specifys an action to take
                        //parameter - the part of the messag that may be passed to a function as a parameter
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


                        if (command.Equals("GAExit") || command.Equals("System-Shutdown"))
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

        /**
         * sets <Active> to false in order to end the primary loop and cancels any current attempt to
         * fetch messages from Kafka
         */
        public void Kill()
        {
            Active = false;
            source.Cancel();
        }
    }
}
