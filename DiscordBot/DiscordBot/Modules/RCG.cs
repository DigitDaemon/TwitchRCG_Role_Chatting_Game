using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("rcg")]
    public class RCG : ModuleBase<SocketCommandContext>
    {


        [Command]
        public async Task DefaultPingAsync()
        {

            //Context.User;
            //Context.Client;
            //Context.Guild;
            //Context.Message;
            //Context.Channel;

            await ReplyAsync($"TwitchRCG is {Context.Guild.GetUser(163737280874414080).Mention}'s senior project.\n" +
                "You can learn more at the follow links:\n" +
                "Short summery of the game: https://tinyurl.com/yyfuvn4d \n" +
                "More technical summery: https://tinyurl.com/yy3m2rge \n" +
                "Github repo: https://github.com/DigitDaemon/TwitchRCG_Role_Chatting_Game");
        }


    }
}
