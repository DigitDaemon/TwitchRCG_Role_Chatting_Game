using System;

/*About: Database Application
 * 
 * This application is part of my senior project. It takkes in messages off of a Kafka server and porforms
 * analysis based on a machine learning algorithm to determine whether a user's message is Angry(Temper), Happy(Cheer),
 * Curious(Curiosity), Cool(Charisma) or Sad(Empathy) and then records it in a PostgreSQL database as part of an entry
 * for that user.
 * 
 * It also filters out messages from twitch made by users not participating and sends an instruction to the <TwitchClientProducer>
 * application to filter out those messages earlier in the system.
 */
namespace DatabaseApplication
{
    /**
     * This is the entry point for the <DatabaseApplication> and manages the console intput loop.
     */
    class Program
    {

        /**
         * The main function for <DatabaseApplication>, holds the console input loop
         */
        static void Main(string[] args)
        {
            //bool: active
            //maintains the console input loop
            bool active = true;
            
            //obj: controller
            //The object that maintains and controlls the <DatabaseApplication> threads
            ThreadController controller = new ThreadController();

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
