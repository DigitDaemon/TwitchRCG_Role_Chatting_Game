using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("rules")]
    public class Rules : ModuleBase<SocketCommandContext>
    {
       

            [Command]
            public async Task DefaultRulesAsync()
            {

                //Context.User;
                //Context.Client;
                //Context.Guild;
                //Context.Message;
                //Context.Channel;

                await ReplyAsync("The Rules for this server are simple: \n" +
                    "Be kind.\n" +
                    "No illegal activity.\n" +
                    "No NSFW content.\n" +
                    "Fun only allowed on Tuesdays.");
            }

        
    }
}
