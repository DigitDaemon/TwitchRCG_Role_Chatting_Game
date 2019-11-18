using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameApplication
{
    /**
     * This class produces to the Kafka server either the 'twitch_out' of 'dicord_out' topics.
     */
    class KafkaProducer
    {


        //vars: Kafka producer configs
        //
        //server - the Kafka server to publish to
        //config - an object containing the configuration for the kafka client
        static string server = "Localhost:9092";
        ProducerConfig config;

        //objs: Message Queues
        //
        //twitchOutQueue - the queue of messages to send back to twitch chat and discord
        //discordOutQueue - the queue of message to only send to discord
        ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue;

        //bool: Active
        //controls the primary loop of <KafkaProducer>
        private bool Active;

        /**
         * The constructor for <KafkaProducer>
         * 
         * Parameters:
         * 
         * messageQueue - the queue shared between <CommandProducer> the <TwitchCommandConsumer> 
         * and the <TwitchMessageConsumer>
         */
        public KafkaProducer(ref ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue, ref ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue)
        {
            this.twitchOutQueue = twitchOutQueue;
            this.discordOutQueue = discordOutQueue;
            //obj: config
            //an object containing the configuration settings for the Kafka client
            config = new ProducerConfig { BootstrapServers = server };
        }

        /**
         * This is the primary function of <KafkaProducer> to be run as a thread
         */
        public void ProducerThread()
        {
            Console.WriteLine("Producer Thread start");
            this.Active = true;
            //int: count
            //the number of times the function loops before voleentarily passing control
            int count = 0;
            //obj: producer
            //the Kafka producer client 
            var producer = new ProducerBuilder<string, string>(config).Build();

            while (this.Active)
            {
                if (!twitchOutQueue.IsEmpty)
                {

                    KeyValuePair<string, string> message;
                    twitchOutQueue.TryDequeue(out message);
                    producer.ProduceAsync("twitch_out", new Message<string, string> { Key = message.Key, Value = message.Value }).Wait();

                }
                else if(!discordOutQueue.IsEmpty)
                {
                    KeyValuePair<string, string> message;
                    discordOutQueue.TryDequeue(out message);
                    producer.ProduceAsync("discord_out", new Message<string, string> { Key = message.Key, Value = message.Value }).Wait();
                }
                else
                {
                    Thread.Yield();
                }

                if (count > 60)
                {
                    Thread.Yield();
                    count = 0;
                }

                count++;
            }
        }

        /**
         * Ends the loop allowing the thread to close
         */
        public void Kill()
        {
            Active = false;
        }

    }
}
