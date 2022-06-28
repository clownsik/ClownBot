using Discord;
using Discord.Commands;

namespace DiscordBot.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("clear", RunMode = RunMode.Async)]
        public async Task ClearAsync(IUserMessage msg)
        {
            Console.WriteLine("Команда CLEAR");

            int amount = 3;
            const int delay = 3000;

            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            await Task.Delay(delay);
            await ReplyAsync($"Удалено {messages.Count()} сообщений :)");
        }

        [Command("hi")]
        public async Task PingAsync()
        {
            Console.WriteLine("Команда HI");
            await ReplyAsync($"ПРИВЕТ, {Context.User.Username}!"); ;
        }

        [Command("purge")]   
        public async Task PurgeAsync()
        {
            Console.WriteLine("Команда PURGE");
            int amount = 100;
            // загрузка сообщений (Context.Message - сообщение вызванное для этой команды останется)
            var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount).FlattenAsync();
            // прверка чтобы сообщения были не старше 14 дней,
            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            // получение общее количество сообщений
            var count = filteredMessages.Count();

            // проверка если ли вообще сообщения
            if (count == 0)
                await ReplyAsync("Сообщений для удаления нет");
            else
            {
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
                await ReplyAsync($"Удалено {count}");
            }
        }
    }
}

