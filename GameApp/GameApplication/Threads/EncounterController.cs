using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace GameApplication
{
    class EncounterController
    {
        string channel;
        Encounter encounter;
        System.Timers.Timer joinPeriod;
        ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue;
        ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue;
        List<Abstracts.Agent> Players;
        bool open;
        public bool gameOver;

        public EncounterController(string channel, ref ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue, ref ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue)
        {
            this.channel = channel;
            this.twitchOutQueue = twitchOutQueue;
            this.discordOutQueue = discordOutQueue;
            Players = new List<Abstracts.Agent>();
            open = true;
            gameOver = false;

            joinPeriod = new System.Timers.Timer(1000);//change this back
            joinPeriod.Elapsed += closeJoin;
            joinPeriod.Enabled = true;
            Console.WriteLine("EncounterController created" + channel);
        }

        public void closeJoin(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("closeJoin" + channel);
            open = false;
            joinPeriod.Elapsed -= closeJoin;
            joinPeriod.Dispose();
        }

        public void encounterThread()
        {
            Console.WriteLine("EncounterThread start" +channel);
            while (open)
            {
                Thread.Sleep(500);
            }
            encounter = SimpleEncounterBuilder.BuildEncounter(Players);
            while (!gameOver)
            {
                try
                {
                   var messageList = encounter.nextTurn();
                   foreach(string message in messageList)
                    {
                        discordOutQueue.Enqueue(new KeyValuePair<string,string>(channel,message));
                    }
                }
                catch(Exceptions.GameOverException go)
                {
                    gameOver = true;
                    foreach(string message in go.getMessages())
                    {
                        discordOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
                    }
                }
            }
            twitchOutQueue.Enqueue(new KeyValuePair<string,string>(channel,encounter.endEncounter()));

        }

        public void enqueTwitch(string message)
        {
            twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
        }

        public void enqueDiscord(string message)
        {
            discordOutQueue.Enqueue(new KeyValuePair<string, string>(channel, message));
        }

        public void AddPlayer(string uname)
        {
            if (open)
            {
                if (!Players.Exists(x => x.getName().Equals(uname))) {
                    try
                    {
                        Players.Add(Builders.CharacterBuilder.buildCharacter(uname));
                    }
                    catch (Exceptions.NoSuchPlayerException nsp)
                    {
                        twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, "@" + uname + " You are not currently registered in TwitchRCG. "
                            + "You can register by typing '!dJoin' into chat."));
                    }
                }
            }
        }
    }
}
