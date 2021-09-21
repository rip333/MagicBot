using System;
using System.Threading;
using System.Threading.Tasks;
using MagicBot.Managers;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace MagicBot
{
    class Program
    {
        private static TelegramBotClient? Bot;

        static async Task Main(string[] args)
        {
            Bot = new TelegramBotClient(TokenManager.Get());
            var me = await Bot.GetMeAsync();
            Console.Title = me.Username ?? "Telegram Bot";

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(
                new DefaultUpdateHandler(ChatManager.HandleUpdateAsync, ChatManager.HandleErrorAsync),
                cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}