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
            Console.WriteLine("ControlLoop start");
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
            Console.WriteLine("AddJoinGame" + message);
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();
            if (encounters.Exists(x => x.Key == channel))
            {
                encounters.Find(x => x.Key == channel).Value.AddPlayer(uname);
            }
            else if(!cooldownList.Exists(x => x.Key == channel)) { 
                    startEncounter(message);
            }
            

            
        }

        public void startEncounter(string message)
        {
            
            var channel = message.Substring(0, message.IndexOf(" "));
            var uname = message.Substring(message.IndexOf(" "), message.Length - channel.Length).Trim();
            Console.WriteLine("Encounter started for " + channel);
            addCooldown(channel);

            KeyValuePair<string, EncounterController> newEncounter = new KeyValuePair<string, EncounterController>(channel, new EncounterController(channel, ref twitchOutQueue, ref discordOutQueue));
            Thread encounterThread = new Thread(newEncounter.Value.encounterThread);
            encounterThread.Start();
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
            for(int i = 0; i < cooldownList.Count; i++)
            {
                if (!cooldownList[i].Value.Enabled)
                {
                    cooldownList[i].Value.Elapsed -= cooldownEnded;
                    cooldownList[i].Value.Dispose();
                    if (encounters.Exists(x => x.Key.Equals(cooldownList[i].Key)) && encounters.Exists(x => x.Value.gameOver))
                    {
                        encounters.Remove(encounters.Find(x => x.Key.Equals(cooldownList[i].Key)));
                    }
                    twitchOutQueue.Enqueue(new KeyValuePair<string, string>(cooldownList[i].Key, "There is a new Quest posted! Start your adventure by typing in '!dQuest' into chat."));
                    cooldownList.Remove(cooldownList.Find(x => x.Key.Equals(cooldownList[i].Key)));
                   
                    break;
                }
            }
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
            active = false;
            commandConsumer.Kill();
            kafkaProducer.Kill();
            gameConsumer.Kill();
            System.Environment.Exit(0);
        }
    }
}
