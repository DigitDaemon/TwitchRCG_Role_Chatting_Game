using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatabaseApplication
{
    /**
     * This class maintains the threads for the <DatbaseAPplication> and handles starting and shutting them down
     */
    class ThreadController
    {
        //objs: Thread objects
        //
        //MConsumer - the object for <TMCon>
        //CConsumer - the object for <TCon>
        //CommandCon - the object for <COMCon>
        //CommandPro - the object for <COMPro>
        TwitchMessageConsumer MConsumer;
        TwitchCommandConsumer CConsumer;
        CommandConsumer CommandCon;
        CommandProducer CommandPro;
        
        //Objs: Threads
        //
        //TMCon - reads from the twitch_message topic on Kafka, analysis user messages and updates the database
        //TCCon - reads from the twitch_command topic on Kafka and adds new users to the database
        //COMcon - reads from the COMMANDS topic on Kafka and exicutes relevent commands
        //COMPro - produces to the COMMANDS topic on Kafka for IPC
        Thread TMCon;
        Thread TCCon;
        Thread COMCon;
        Thread COMPro;

        //obj: commandMessageQueue
        //shared by the <CommandProducer> the <TwitchCommandConsumer> and the <TwitchMessageConsumer>
        ConcurrentQueue<string> commandMessageQueue;

        /**
         * The constructor for <ThreadController>
         * Initialises all objs and threads needed for the <DatabaseApplication> to run
         */
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

        /**
         * Provides feedback on the state of the threads in <DatabaseApplication>
         */
        public void listThreads()
        {
            Console.WriteLine(TMCon.ThreadState);
            Console.WriteLine(TCCon.ThreadState);
            Console.WriteLine(COMCon.ThreadState);
            Console.WriteLine(COMPro.ThreadState);
        }

        /**
         * Closes all of the threads and then ends the program
         */
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
