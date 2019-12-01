using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) 
            => new Program().RunBotAsync().GetAwaiter().GetResult();
        

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private static string path = Path.Combine(Environment.CurrentDirectory.Replace(@"bin\Debug\netcoreapp2.1", ""), @"Data\");
        private static string botToken = File.ReadAllText(Path.Combine(path, "Token.txt"));

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //event subscriptions
            _client.Log += Log;
            _client.UserJoined += AnnounceUserJoinedAsync;

            ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
            DiscordOutConsumer DiscordConsumer = new DiscordOutConsumer(ref messageQueue); 
            Thread DiscordThread = new Thread(DiscordConsumer.DisConThread);
            DiscordThread.Start();
            
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await QueueMessageAsync(messageQueue);

            await Task.Delay(-1);
        }

        private async Task QueueMessageAsync(ConcurrentQueue<string> messageQueue)
        {
            while (true)
            {
                if (!messageQueue.IsEmpty)
                {
                    var msg = "";
                    msg.Replace("\0","");
                    msg.Replace(":", "");
                    messageQueue.TryDequeue(out msg);
                    var channel = msg.Substring(0,msg.IndexOf(" "));
                    var message = msg.Substring(msg.IndexOf(" "), msg.Length - channel.Length).Trim();
                    ulong id = 650480545280688158;
                    var chnl = _client.GetChannel(id) as IMessageChannel;
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.AddField(channel, message)
                        .WithColor(Color.DarkRed);
                     
                    await chnl.SendMessageAsync("", false, builder.Build());
                }
            }
            
        }

        private async Task AnnounceUserJoinedAsync(SocketGuildUser user)
        {
            ulong id = 650480622174994442;
            var guild = user.Guild;
            var channel = _client.GetChannel(id) as IMessageChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }



        }

    }
}
