using System;
using System.Threading;

namespace GameApplication
{
    /**
     * This is the entry point for the <GameApplication> and manages the console intput loop.
     */
    class Program
    {

        /**
         * The main function for <GameApplication>, holds the console input loop
         */
        static void Main(string[] args)
        {
            //bool: active
            //maintains the console input loop
            bool active = true;

            //obj: controller
            //The object that maintains and controlls the <GameApplication> threads
            ThreadController controller = new ThreadController();
            Thread conThread = new Thread(controller.ControlLoop);
            conThread.Start();

            while (active)
            {
                //strings: Input
                //
                //input - the input from the console
                //command - the portion of the input that determines which instruction to carry out
                //parameter - the part of a command that may be passed into a function as a parameter
                var input = Console.ReadLine();
                string command;
                string parameter;

                if (input.Contains(" "))
                {
                    command = input.Substring(0, input.IndexOf(" ")).Trim();
                    parameter = input.Substring(input.IndexOf(" "), input.Length - 1).Trim();
                }
                else
                {
                    command = input;
                    parameter = "";
                }

                Console.WriteLine("COMMAND----------> " + input);

                if (command.Equals("Exit"))
                {
                    controller.exit();
                    active = false;
                }
                else if (command.Equals("List-Threads"))
                {
                    controller.listThreads();
                }
            }
        }
    }
}
