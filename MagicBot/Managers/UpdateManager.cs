using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MagicBot.Models.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MagicBot.Managers
{
    public class UpdateManager
    {
        private ScryfallApi _scryfallApi;

        public UpdateManager(ScryfallApi scryfallApi)
        {
            _scryfallApi = scryfallApi;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException =>
                    $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;
            if (update.Message != null && update.Message.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;

            Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

            if (ChatUtility.ShouldReturnImageUri(update.Message.Text))
            {
                try
                {
                    var cardNames = ChatUtility.GetCardNamesInMessage(update.Message.Text);
                    if (cardNames.Count == 1)
                    {
                        var cardUri = await _scryfallApi.GetCardImageUrlByName(cardNames[0]);
                        await botClient.SendPhotoAsync(chatId: chatId, photo: cardUri,
                            cancellationToken: cancellationToken);
                    }
                    else if (cardNames.Count > 1)
                    {
                        var inputMedia = new List<IAlbumInputMedia>();
                        foreach (var cardName in cardNames)
                        {
                            var cardUri = await _scryfallApi.GetCardImageUrlByName(cardName);
                            inputMedia.Add(new InputMediaPhoto(cardUri));
                        }

                        await botClient.SendMediaGroupAsync(chatId, inputMedia, cancellationToken: cancellationToken);
                    }
                }
                catch (NoCardFoundException e)
                {
                    Console.WriteLine($"No card found exception in message: {update.Message.Text}");
                }
            }
        }
    }
}