using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace StreamClientProducer
{

    /**
     * This class is responcible for dequeue messages from <messageQueue> and then publishing them to the
     * kafka server.
     */
    class KafkaProducer
    {
        /**
         * The shared queue that contains all the incoming messages from twitch
         */
        ConcurrentQueue<String> messageQueue;

        /**
         * manages the main loop for this object
         */
        private bool Active;

        //vars: Kafka Connection Objects
        //
        //server - the server to connect to
        //config - the object containing configuration information
        const string server = "Localhost:9092";
        ProducerConfig config;

        /**
         * The coinstructor for <KafkaProducer>
         * 
         * Parameter:
         * messageQueue - a refrence to the the queue shared by all the <TwitchClients> 
         */
        public KafkaProducer(ref ConcurrentQueue<String> messageQueue)
        {
            this.messageQueue = messageQueue;
            config = new ProducerConfig { BootstrapServers = server };
            


        }

        /**
         * This is the primary fuction and run as a thread by <ThreadController>
         */
        public void ProducerThread()
        {
            Console.WriteLine("Producer Thread start");
            this.Active = true;
            int count = 0;
            
            /**
             *The kafka producer that will publish to the twitch_message and twitch_command topics
             */
            var producer = new ProducerBuilder<string, string>(config).Build();

            while (this.Active)
            {
                if (!messageQueue.IsEmpty)
                {
                    string message;
                    messageQueue.TryDequeue(out message);
                    string key = message.Substring(0, message.IndexOf(" "));

                    /*
                     * Sort the messages into regular messages and commands for my system.
                     */
                    if (message.Contains("!d"))
                    {
                        producer.ProduceAsync("twitch_command", new Message<string, string> { Key = key, Value = message }).Wait();
                    }
                    else
                    {
                        producer.ProduceAsync("twitch_message", new Message<string, string> { Key = key, Value = message }).Wait();
                    }
                }
                else
                {
                    Thread.Yield();
                }

                if (count > 60)
                {
                    Thread.Sleep(500);
                    count = 0;
                }

                count++;
            }
        }

        /**
         * This function closes the thread after it has finished transmitting all messages in the queue to Kafka
         */
        public async Task KillAsync()
        {
                while (messageQueue.Count > 0)
                {
                    Console.WriteLine(messageQueue.Count);
                }
                this.Active = false;

        }
        
    }
}
