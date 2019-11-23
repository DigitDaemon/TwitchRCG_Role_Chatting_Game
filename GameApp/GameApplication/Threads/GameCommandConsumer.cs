using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameApplication
{
    /**
     * This class reads in messages from the 'twitch_command' Kafka topic and seperates them into game commands and other commands
     */
    class GameCommandConsumer
    {
        //vars: Kafka Consumer Config
        //
        //server - the kafka server to connect to
        //topic - the topic to subscribe to
        //config - an object containing configuration information for the Kafka connection
        //cancelToken - a token used to cancel a the Kafka consumer looing for new messages
        //source - the source of <cancelToken>
        static string server = "Localhost:9092";
        string topic = "twitch_command";
        ConsumerConfig config;
        CancellationToken canceltoken;
        CancellationTokenSource source;

        //objs: Queues
        //
        //gameMesssageQueue - the queue for commands relating to starting or playing the twitch game
        //otherMessageQueue - the queue for other commands still relevent to the application
        ConcurrentQueue<string> gameMessageQueue;
        ConcurrentQueue<string> otherMessageQueue;

        //objs:
        //
        //GAME_START_FILTER - a list of commands that will start or join an instance of the game
        //OTHER_MESSAGE_FILTER - a list of the other relevent commands
        private static List<string> GAME_START_FILTER = new List<string>() { "!dquest" };
        private static List<string> OTHER_MESSAGE_FILTER = new List<string>() { "placeholder" };

        //bool: Active
        //Controls the main loop in <GameCommandThread>
        private bool Active;

        /**
         * the constructor for <GameCommandConsumer>
         * 
         * Parameters:
         * 
         * gameMessageQueue - the refrence to a queue created by <ThreadController> for passing messages between threads
         * otherMessageQueue - the refrence to a queue created by <ThreadController> for passing messages between threads
         */
        public GameCommandConsumer(ref ConcurrentQueue<string> gameMessageQueue, ref ConcurrentQueue<string> otherMessageQueue)
        {
            this.gameMessageQueue = gameMessageQueue;
            this.otherMessageQueue = otherMessageQueue;

            config = new ConsumerConfig
            {
                BootstrapServers = server,
                GroupId = Guid.NewGuid().ToString(),
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnablePartitionEof = true
            };
            source = new CancellationTokenSource();
            canceltoken = source.Token;
        }

        /**
         * the primary function of <GameCommandConsumer> to be run as a thread
         */
        public void GameCommandThread()
        {
            Console.WriteLine("CommandConsumerThread start.");
            Active = true;


            var consumer = new ConsumerBuilder<string, string>(config).Build();
            var topicp = new TopicPartition(topic, 0);
            consumer.Assign(topicp);

            while (Active)
            {
                try
                {
                    var consumeresult = consumer.Consume(canceltoken);

                    if (!consumeresult.IsPartitionEOF)
                    {
                        //strings: Message
                        //
                        //input - the raw message
                        //command - the portion of the message containing a command
                        //username - the user name of the person who sent the command
                        //channel - the channel that the command was sent from
                        var input = consumeresult.Value;
                        string command;
                        string username;
                        string channel;


                        channel = input.Substring(0, input.IndexOf(" ")).Trim();
                        command = input.Substring(input.IndexOf(" "), input.Length - channel.Length).Trim();
                        username = command.Substring(0, command.IndexOf(" ")).Trim();
                        command = command.Substring(command.IndexOf(" "), command.Length - username.Length).Trim();
                        username.TrimStart(new char[] { '\0', ':' });

                        Console.WriteLine("GameCommand----------> " + input);

                        if (GAME_START_FILTER.Contains(command.ToLower()))
                        {
                            gameMessageQueue.Enqueue(channel + " " + username);
                            Console.WriteLine(username + "tried to join");
                        }
                        else if (OTHER_MESSAGE_FILTER.Contains(command.ToLower()))
                        {
                            otherMessageQueue.Enqueue(input);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (System.OperationCanceledException e)
                {

                }

            }

        }

        /**
         * sets <Active> to false in order to end the primary loop and cancels any current attempt to
         * fetch messages from Kafka
         */
        public void Kill()
        {
            Active = false;
            source.Cancel();
        }
    }
}
