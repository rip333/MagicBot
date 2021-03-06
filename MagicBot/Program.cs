using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MagicBot.Managers;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace MagicBot
{
    class Program
    {
        private static TelegramBotClient _bot;
        private static UpdateManager _updateManager;

        static async Task Main(string[] args)
        {
            var token = await ConfigUtility.GetToken();

            //Services Collection Setup
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.scryfall.com/")
            };
            var scryfallClient = new ScryfallClient(httpClient);
            var scryfallApi = new ScryfallApi(scryfallClient);
            _updateManager = new UpdateManager(scryfallApi);

            //Bot Setup
            _bot = new TelegramBotClient(token);
            var me = await _bot.GetMeAsync();
            Console.Title = me.Username ?? "Telegram Bot";
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            _bot.StartReceiving(
                new DefaultUpdateHandler(_updateManager.HandleUpdateAsync, _updateManager.HandleErrorAsync),
                cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.Read();
            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}