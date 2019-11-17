using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StreamClientProducer
{
    /**
     *This class controls all the threads in this application and handles coordination between them.
     */
    class ThreadController
    {
        /**
         *All messages that come in from the irc clients will be placed into this queue. 
         * It is shard with all <ClientThread> objects and the <TwitchProducer> class.
         */ 
        ConcurrentQueue<String> messageQueue;
        /** 
         * This timer triggers the event for each fo the <CLientThread> threads to check for a new message.
         */
        static System.Timers.Timer trigger;
        // objs: Twitch Clients
        // 
        // Channels - A list of all the channels that currently have a <TwitchClient> assosiated with them
        // Clients - Holds all the <TwitchClient> instances.
        List<string> Channels;
        List<TwitchClient> Clients;
        // objs: Kafka Clients
        // 
        // producer - The <KafkaProducer> that will publish all of the incoming messages to the Kafka server.
        // pro - The thread assosiated with <producer>
        // 
        // command - the <CommandConsumer> that subscribes to the COMMANDS topic on the Kafka server for IPC
        // com - the thread assosiated with <command>
        KafkaProducer producer;
        CommandConsumer command;
        Thread pro;
        Thread com;
        /** 
         *this is a set of all the users that are blocked from having messages read in <producer>
         */
        HashSet<string> blacklist;

        /** 
         * The constructor for <ThreadController>
         * Initializes the above objects, starts the <trigger> timer and starts the <pro> and <com> threads
         */
        public ThreadController()
        {
            blacklist = new HashSet<string>();
            trigger = new System.Timers.Timer(100);
            trigger.AutoReset = true;
            trigger.Enabled = true;
            messageQueue = new ConcurrentQueue<String>();
            Channels = new List<string>();
            
            Clients = new List<TwitchClient>();
            
            producer = new KafkaProducer(ref messageQueue);
            pro = new Thread(producer.ProducerThread);
            pro.Name = "PRODUCER";
            pro.Start();
            command = new CommandConsumer(this, ref blacklist);
            com = new Thread(command.CommandThread);
            com.Name = "Command";
            com.Start();
        }

        /** 
         * Creates a new TwitchClient and adds it to <clients>
         * 
         * Parameter: 
         * 
         * channel - The name of the channel to create a client for.
         */
        public void addClient(string channel)
        {
            if (!channel.Equals(""))
            {
                Channels.Add(channel);
                int index = Channels.FindIndex(a => a.Equals(channel));
                Clients.Add(new TwitchClient(channel, ref trigger, ref messageQueue, ref blacklist));
                Clients[index].CThread();
            }
            else
            {
                Console.WriteLine("Parrameter needed to exicute command Add-Channel");
            }
        }
        /**
         * Removes a client from the application
         * 
         * Parameter:
         * channel - The name of the channel that should have it's client dropped.
         */
        public void dropClient(string channel)
        {
            if (!channel.Equals(""))
            {
                try
                {
                    int index = Channels.FindIndex(a => a.Equals(channel));
                    Clients[index].Kill(); 
                    Channels.RemoveAt(index);
                    Clients.RemoveAt(index);
                    Console.WriteLine(channel + " dropped");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.WriteLine("Parameter needed to exicute Drop-Channel command");
            }
        }

        /**
         * Prints a list of the currently running channels to the console
         */
        public void listThreads()
        {
            
            foreach(string channel in Channels)
            {
                Console.WriteLine(channel);
            }
            Console.WriteLine("kafka producer" + pro.ThreadState);
            Console.WriteLine("command consumer" + com.ThreadState);
        }

        /**
         * Prints the amount of messages that are waiting to be published to Kafka.
         */
        public int queueSize()
        {
            return messageQueue.Count;
        }

        /**
         * Shuts down all of the threads and then closes the application.
         */
        public void exit()
        {
            while(Channels.Count > 0)
            {
                dropClient(Channels[0]);
            }
            pro.Priority = ThreadPriority.Highest;
            producer.KillAsync().Wait();
            command.Kill();
            System.Environment.Exit(0);
        }

        /**
         * Lists all of the users that are curently in <blacklist> 
         */
        public void listBlacklist()
        {
            foreach(string name in blacklist)
            {
                Console.WriteLine("The blocked users are:");
                Console.WriteLine(name);
            }
        }
    }
}
