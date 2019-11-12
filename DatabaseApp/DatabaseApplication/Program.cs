using System;

namespace DatabaseApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            bool active = true;
            ThreadController controller = new ThreadController();
            while (active)
            {
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
