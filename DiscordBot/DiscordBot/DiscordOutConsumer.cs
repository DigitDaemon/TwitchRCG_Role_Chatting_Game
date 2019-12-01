using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace DiscordBot
{
    public class DiscordOutConsumer
    {
        //vars: Kafka Consumer Config
        //
        //server - the kafka server to connect to
        //topic - the topic to subscribe to
        //config - an object containing configuration information for the Kafka connection
        //cancelToken - a token used to cancel a the Kafka consumer looing for new messages
        //source - the source of <cancelToken>
        static string server = "Localhost:9092";
        string topic = "discord_out";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //bool: Active
        //controls the primary loop in <TCCThread>
        private bool Active;

        //var: commandeMessageQueue
        //The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
        //this is shared by the <CommandProducer> the <TwitchCommandConsumer> and the <TwitchMessageConsumer>
        ConcurrentQueue<string> messageQueue;


        /**
         * The constructor for <TwitchCommandConsumer>
         * 
         * Parameters:
         * 
         * commandMessageQueue - The queue for outgoing messages from this applicaiton to be placed in for publishing by <CommandProducer>
         */
        public DiscordOutConsumer(ref ConcurrentQueue<string> messageQueue)
        {
            this.messageQueue = messageQueue;

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
         * This is the primary function of <TwitchCommandConsumer> and is to be run as a thread
         */
        public void DisConThread()
        {
            Console.WriteLine("DiscordMessageConsumerThread Start");
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
                        messageQueue.Enqueue(consumerresult.Key + " " + consumerresult.Value);
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
            Active = false;
            source.Cancel();
        }
    }
}
