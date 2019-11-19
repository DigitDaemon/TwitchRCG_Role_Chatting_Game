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
        bool open;

        public EncounterController(string channel, ref ConcurrentQueue<KeyValuePair<string, string>> twitchOutQueue, ref ConcurrentQueue<KeyValuePair<string, string>> discordOutQueue)
        {
            this.channel = channel;
            this.twitchOutQueue = twitchOutQueue;
            this.discordOutQueue = discordOutQueue;
            open = true;

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
                //add player to encounter
            }
        }
    }
}
