using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace DiscordBot
{
    class Program
    {
        DiscordSocketClient client;
        IServiceProvider services;
        CommandService commands;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            //var token = "OTg5MDg3MzAyNDc3MjkxNTIw.GZ6-ht.eXXUVjpmGFMsbdufJSkVYlsmqTlgbQ80_UBUy8";

            var tt = File.ReadAllText(@"D:\source\DsBot\DiscordBot\token.json");
            var json = JObject.Parse(tt);
            var t = json["token"].ToString();

            client.Log += Log;

            await RegisterCommandsAsync();
            await client.LoginAsync(TokenType.Bot, t);
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        public async Task RegisterCommandsAsync()
        {
            commands.CommandExecuted += OnCommandExecutedAsync;
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            var context = new SocketCommandContext(client, msg);
            if (msg.Author.IsBot) return;

            Console.WriteLine("Команда: " + msg.Content);

            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
            {
                var res = await commands.ExecuteAsync(context, argPos, services);
                if (!res.IsSuccess) Console.WriteLine(res.ErrorReason);
                if (res.Error.Equals(CommandError.UnmetPrecondition)) await msg.Channel.SendMessageAsync(res.ErrorReason);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}