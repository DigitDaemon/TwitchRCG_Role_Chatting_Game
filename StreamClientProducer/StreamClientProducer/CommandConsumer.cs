using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace StreamClientProducer
{
    /**
     * This class provides the IPC functionality by subscribing to the COMMANDS topic on the Kafka server
     */
    class CommandConsumer
    {
        //Consts: Kafka Connection Information
        //
        //server - The server information
        //topic - The Kafak topic to subscribe to
        const string server = "Localhost:9092";
        const string topic = "COMMANDS";

        //var: Active
        //
        //Manages the main loop in <CommandThread>
        private bool Active;

        //obj: controller
        //
        //The <ThreadController> that instanciated this obj.
        //Access is necesary to allow IPC commands to propagate through this Application
        ThreadController controller;

        //obj: config
        //
        //The configuration for the Kafka client
        ConsumerConfig config;

        //objs: Cancellation
        //
        //canceltoken - a taken passed to the Kafka client
        //source - the source for <canceltoken>
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //obj: blacklist
        //
        //a list of users to ignore messages from, passed in from <THreadController>
        HashSet<string> blacklist;

        /**
         * The constructor for <CommandConsumer>
         * Generates the configuraiton for the Kafka client.
         * 
         * Parameters:
         * controller - the <ThreadConstroller> that created this object
         * blacklist - a refrence to the shared <blacklist> object owned by <ThreadConstroller>
         */
        public CommandConsumer(ThreadController controller, ref HashSet<string> blacklist)
        {
            this.blacklist = blacklist;
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
         * The main loop to be run as a thread. Generates a Kafka client that checks 
         * for new messages COMMANDS and then calls the appropriete function from <THreadController>
         */
        public void CommandThread()
        {
            Console.WriteLine("CommandThread Start");
            Active = true;
            //Objs: Kafka Client
            //
            //consumer - the Kafka consumer that will connect to the Kafka server
            //topicp - what partition to start looking for messages in.
            var consumer = new ConsumerBuilder<string, string>(config).Build();
            var topicp = new TopicPartition(topic, 0);
            consumer.Assign(topicp);

            while (Active)
            {
                try
                {
                    /**
                     * the result of <consumer> checking for nes messages
                     */
                    var consumeresult = consumer.Consume(canceltoken);

                    if (!consumeresult.IsPartitionEOF)
                    {
                        /*
                         * This section breaks down the message and then exicutes the appropriete command
                         */

                        //Vars: Commad Strings
                        //
                        //input - the raw message
                        //command - the part of the message that specifies what function to exicute
                        //parameter - the part of a message that may be passed into a function at exicution
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

                        if (command.Equals("Add-Channel"))
                        {
                            controller.addClient(parameter);
                        }
                        else if (command.Equals("Drop-Channel"))
                        {
                            controller.dropClient(parameter);
                        }
                        else if (command.Equals("SCPList-Channels"))
                        {
                            Console.WriteLine("The active threads are:");
                            controller.listThreads();
                        }
                        else if (command.Equals("SCPCount"))
                        {
                            Console.WriteLine("The message queue has " + controller.queueSize() + " messages in it right now");

                        }
                        else if (command.Equals("SCPExit") || command.Equals("System-Shutdown"))
                        {
                            controller.exit();
                            Active = false;
                            source.Cancel();
                        }
                        else if (command.Equals("SCPBlacklist"))
                        {
                            blacklist.Add(parameter);
                        }
                        else if (command.Equals("SCPUnblacklist"))
                        {
                            blacklist.Remove(parameter);
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
         * Ends the primary loop and allows the thread to end.
         */
        public void Kill()
        {
            source.Cancel();
            Active = false;
        }

    }

}
