using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("clear", RunMode = RunMode.Async)]
        public async Task ClearAsync()
        {
            int amount = 100;
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            const int delay = 3000;
            await Task.Delay(delay);
            await ReplyAsync($"Удалено {messages.Count()} сообщений :)");
        }

        [Command("hi")]
        public async Task PingAsync()
        {
            Console.WriteLine("Команда пинг-понг");
            await ReplyAsync($"ПРИВЕТ, {Context.User.Username}!"); ;
        }
    }
}
