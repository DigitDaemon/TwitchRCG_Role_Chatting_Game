using System;

/**
 * About: TwitchRCG StreamClientProducer
 * This Application is the first part of the system I am build for my senior project.
 * This application takes in message from Twitch through irc and sorts them into messagtes and commands
 * before sending them into Kafka to be distributed to the rest of my system.
 * 
 * This applicaiton is interactable both through the comand line and through a Kafka topic, 
 * allowing for interapplicaiton communication. 
 */

namespace StreamClientProducer
{
    //Class: Program
    //This class launches the ThreadController and provides the console facing interactivity 
    //for the application.
    class Program
    {
        static ThreadController controller;
        
    

        //Function: Main
        //The main method, contains an input loop for console interactivity
        static void Main(string[] args)
        {
            
            bool active = true; //This bool keeps the loop going until the user chooses to exit
            controller = new ThreadController(); //This class will handle all of the functionality of the Application
            while (active)
            {
                var input = Console.ReadLine();
                string command;
                string parameter;
                if (input.Contains(" "))//this code block seperates commands with multiple qords into a command and a parameter
                {
                    command = input.Substring(0, input.IndexOf(" ")).Trim();
                    parameter = input.Substring(input.IndexOf(" "), input.Length - command.Length).Trim();
                }
                else
                {
                    command = input;
                    parameter = "";
                }

                Console.WriteLine("COMMAND----------> " + input);

                if (command.Equals("Add-Channel")){
                    controller.addClient(parameter);
                }
                else if (command.Equals("Drop-Channel"))
                {
                    controller.dropClient(parameter);
                }
                else if (command.Equals("List-Channels"))
                {
                    Console.WriteLine("The active threads are:");
                    controller.listThreads();
                }
                else if (command.Equals("Count"))
                {
                    Console.WriteLine("The message queue has " + controller.queueSize() + " messages in it right now");
                    
                }
                else if (command.Equals("Exit"))
                {
                    controller.exit();
                    active = false;
                }
                else if (command.Equals("Blacklist"))
                {
                    controller.listBlacklist();
                }
            }

        }

    }
}
