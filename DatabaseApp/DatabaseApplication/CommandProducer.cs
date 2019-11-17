using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace DatabaseApplication
{
    /**
     * This class will produce commands to the Kafka topic COMMANDS to facilitate IPC
     */
    class CommandProducer
    {
        //vars: Kafka producer configs
        //
        //server - the Kafka server to publish to
        //config - an object containing the configuration for the kafka client
        static string server = "Localhost:9092";
        ProducerConfig config;

        //var: messageQueue
        //the queue of messages to publish to Kafka.
        //this is shared by the <CommandProducer> the <TwitchCommandConsumer> and the <TwitchMessageConsumer>
        ConcurrentQueue<string> messageQueue;

        //bool: Active
        //controls the primary loop of <CommandProducer>
        private bool Active;

        /**
         * The constructor for <CommandProducer>
         * 
         * Parameters:
         * 
         * messageQueue - the queue shared between <CommandProducer> the <TwitchCommandConsumer> 
         * and the <TwitchMessageConsumer>
         */
        public CommandProducer(ref ConcurrentQueue<string> messageQueue)
        {
            this.messageQueue = messageQueue;
            //obj: config
            //an object containing the configuration settings for the Kafka client
            config = new ProducerConfig { BootstrapServers = server };
        }

        /**
         * This is the primary function of <CommandConsumer> to be run as a thread
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
                if (!messageQueue.IsEmpty)
                {

                    string message;
                    messageQueue.TryDequeue(out message);
                    producer.ProduceAsync("COMMANDS", new Message<string, string> {Value = message }).Wait();
                    
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
         * Ends the loop allowing the thread to close
         */
        public void Kill()
        {
            Active = false;
        }
    }
}
