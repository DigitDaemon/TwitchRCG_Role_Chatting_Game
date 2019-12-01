using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("ping")]
    public class Ping : ModuleBase<SocketCommandContext>
    {

        [Command]
        public async Task DefaultPingAsync()
        {

            //Context.User;
            //Context.Client;
            //Context.Guild;
            //Context.Message;
            //Context.Channel;

            await ReplyAsync("pong!");
        }

        [Command("user")]
        public async Task PingUserAsync(SocketGuildUser user)
        {

            await ReplyAsync($"Pong! {user.Mention}");
        }

        [Command("test")]
        public async Task PingTestChannel()
        {
            DiscordSocketClient _client = Context.Client;
            ulong id = 615958559892439186;
            var chnl = _client.GetChannel(id) as IMessageChannel;
            await chnl.SendMessageAsync("test this");
        }
    }
}
