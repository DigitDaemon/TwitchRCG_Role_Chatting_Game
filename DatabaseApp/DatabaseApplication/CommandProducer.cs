using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace DatabaseApplication
{

    class CommandProducer
    {
        static string server = "Localhost:9092";
        private bool Active;
        ProducerConfig config;
        ConcurrentQueue<string> messageQueue;

        public CommandProducer(ref ConcurrentQueue<string> messageQueue)
        {
            this.messageQueue = messageQueue;
            config = new ProducerConfig { BootstrapServers = server };
        }

        public void ProducerThread()
        {
            Console.WriteLine("Producer Thread start");
            this.Active = true;
            int count = 0;
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

        public void Kill()
        {
            Active = false;
        }
    }
}
