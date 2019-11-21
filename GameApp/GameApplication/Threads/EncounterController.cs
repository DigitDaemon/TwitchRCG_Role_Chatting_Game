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
        Queue<Agents.Player> Players;
        bool open;
        bool gameOver;

        public EncounterController(string channel, ref ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue, ref ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue)
        {
            this.channel = channel;
            this.twitchOutQueue = twitchOutQueue;
            this.discordOutQueue = discordOutQueue;
            open = true;
            gameOver = false;

            joinPeriod = new System.Timers.Timer(120000);
            joinPeriod.Elapsed += closeJoin;
            joinPeriod.Enabled = true;
        }

        public void closeJoin(Object source, ElapsedEventArgs e)
        {
            open = false;
            joinPeriod.Elapsed -= closeJoin;
            joinPeriod.Dispose();
        }

        public void encounterThread()
        {
            while (open)
            {
                Thread.Sleep(500);
            }
            while (!gameOver)
            {
                try
                {
                    encounter.nextTurn();
                }
                catch(Exceptions.GameOverException go)
                {
                    gameOver = true;
                }
            }
            encounter.endOfEncounter();
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
                try
                {
                    encounter.AddPlayer(SimpleCharacterBuilder.buildCharacter(uname));
                }
                catch(Exceptions.NoSuchPlayerException nsp)
                {
                    twitchOutQueue.Enqueue(new KeyValuePair<string, string>(channel, "@" + uname + " You are not currently registered in TwitchRCG. "
                        + "You can register by typing '!dJoin' into chat."));
                }
            }
        }
    }
}
