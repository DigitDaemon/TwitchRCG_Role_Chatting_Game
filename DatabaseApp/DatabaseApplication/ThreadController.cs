using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatabaseApplication
{
    class ThreadController
    {
        TwitchMessageConsumer MConsumer;
        TwitchCommandConsumer CConsumer;
        CommandConsumer CommandCon;
        CommandProducer CommandPro;
        ConcurrentQueue<string> commandMessageQueue;
        Thread TMCon;
        Thread TCCon;
        Thread COMCon;
        Thread COMPro;

        public ThreadController()
        {
            commandMessageQueue = new ConcurrentQueue<string>();
            MConsumer = new TwitchMessageConsumer(ref commandMessageQueue);
            CConsumer = new TwitchCommandConsumer(ref commandMessageQueue);
            CommandCon = new CommandConsumer(this);
            CommandPro = new CommandProducer(ref commandMessageQueue);
            TMCon = new Thread(MConsumer.TMCThread);
            TMCon.Start();
            TMCon.Priority = ThreadPriority.Highest;
            TCCon = new Thread(CConsumer.TCCThread);
            TCCon.Start();
            COMCon = new Thread(CommandCon.CommandThread);
            COMCon.Start();
            COMPro = new Thread(CommandPro.ProducerThread);
            COMPro.Start();
        }

        public void listThreads()
        {
            Console.WriteLine(TMCon.ThreadState);
            Console.WriteLine(TCCon.ThreadState);
            Console.WriteLine(COMCon.ThreadState);
            Console.WriteLine(COMPro.ThreadState);
        }

        public void exit()
        {
            MConsumer.Kill();
            CConsumer.Kill();
            CommandCon.Kill();
            CommandPro.Kill();
            System.Environment.Exit(0);
        }
    }
}
