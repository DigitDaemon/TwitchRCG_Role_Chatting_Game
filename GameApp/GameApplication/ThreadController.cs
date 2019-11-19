using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace GameApplication
{
    /**
     * This class maintains the threads for the <GameApplication> and handles starting and shutting them down
     */
    class ThreadController
    {


        //bool: active
        //Keeps the loop in <ControlLoop> active
        bool active;

        //objs: Kafka clients
        //
        //commandConsumer - handles receiving IPC messages
        //kafkaProduces - publishes messages to the twitch_out and discord_out Kafka topics
        //gameConsumer - reads in messages from the twitch_command topic
        KafkaCommandConsumer commandConsumer;
        KafkaProducer kafkaProducer;
        GameCommandConsumer gameConsumer;

        //objs: Encounter Lists
        //
        //encounter - a list of ongoing encounters
        //cooldownList - a list of channels that are on cooldown and cannot have a new encounter started yet
        List<KeyValuePair<string, EncounterController>> encounters;
        List<KeyValuePair<string, System.Timers.Timer>> cooldownList;

        //objs: Queues
        //
        //gameMessageQueue - game related messages coming from <GameCommandConsumer>
        //otherMessageQueue - other relevent messages coming from <GameCommandConsumer
        //twitchOutQueue - messages to me passed to <KafkaProducer>
        //discordOutQueue - messages to be passed to <KafakProducer>
        ConcurrentQueue<string> gameMessageQueue;
        ConcurrentQueue<string> otherMessageQueue;
        ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue;

        /**
         * The constructor for <ThreadController>
         * Initialises all objs and threads needed for the <GameApplication> to run
         */
        public ThreadController()
        {
            gameMessageQueue = new ConcurrentQueue<string>();
            otherMessageQueue = new ConcurrentQueue<string>();
            twitchOutQueue = new ConcurrentQueue<KeyValuePair<string, string>>();
            discordOutQueue = new ConcurrentQueue<KeyValuePair<string, string>>();

            commandConsumer = new KafkaCommandConsumer(this);
            kafkaProducer = new KafkaProducer(ref twitchOutQueue, ref discordOutQueue);
            gameConsumer = new GameCommandConsumer(ref gameMessageQueue, ref otherMessageQueue);
            Thread cC = new Thread(commandConsumer.CommandThread);
            cC.Start();
            Thread kP = new Thread(kafkaProducer.ProducerThread);
            kP.Start();
            Thread gC = new Thread(gameConsumer.GameCommandThread);
            gC.Start();

            encounters = new List<KeyValuePair<string, EncounterController>>();
            cooldownList = new List<KeyValuePair<string, System.Timers.Timer>>();
            active = true;
        }

        public void ControlLoop()
        {
            while (active)
            {
                if (!gameMessageQueue.IsEmpty)
                {
                    string message;
                    gameMessageQueue.TryDequeue(out message);
                    AddJoinGame(message);
                }

            }

        }

        public void AddJoinGame(string message)
        {
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();
            try
            {
                encounters.Find(x => x.Key == channel).Value.AddPlayer(uname);
            }
            catch(ArgumentNullException e)
            {
                try
                {
                    var cooldown = cooldownList.Find(x => x.Key == channel);
                }
                catch(ArgumentNullException j)
                {
                    startEncounter(message);
                }
                
            }

            
        }

        public void startEncounter(string message)
        {
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();

            addCooldown(channel);

            KeyValuePair<string, EncounterController> newEncounter = new KeyValuePair<string, EncounterController>(channel, new EncounterController());
            /*
             * Add encounter thread stuff here 
             */
            newEncounter.Value.AddPlayer(uname);
            encounters.Add(newEncounter);
            
        }

        public void addCooldown(string channel)
        {
            KeyValuePair<string, System.Timers.Timer> cooldown = new KeyValuePair<string, System.Timers.Timer>(channel, new System.Timers.Timer(600000));
            cooldown.Value.AutoReset = false;
            cooldown.Value.Enabled = true;
            cooldown.Value.Elapsed += cooldownEnded;
            cooldownList.Add(cooldown);
        }

        public void cooldownEnded(Object source, ElapsedEventArgs e)
        {
            System.Timers.Timer timer = source as System.Timers.Timer;
            timer.Elapsed -= cooldownEnded;
            cooldownList.RemoveAt(cooldownList.FindIndex(x => x.Value == timer));
            timer.Dispose();
        }

        /**
         * Provides feedback on the state of the threads in <DatabaseApplication>
         */
        public void listThreads()
        {
           
        }

        /**
         * Closes all of the threads and then ends the program
         */
        public void exit()
        {
           
        }
    }
}
