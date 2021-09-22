using System;
using System.Collections.Generic;
using System.Linq;
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
            if (update.Message == null)
                return;

            var chatId = update.Message.Chat.Id;

            Console.WriteLine($"Received message in chat {chatId}.");

            if (ChatUtility.ShouldReturnImageUri(update.Message))
            {
                var cardNames = ChatUtility.GetCardNamesInMessage(update.Message.Text);
                if (cardNames.Count == 1)
                {
                    try
                    {
                        var cardUri = await _scryfallApi.GetCardImageUrlByName(cardNames[0]);
                        Console.WriteLine($"Sending photo for card name - {cardNames[0]}.");
                        await botClient.SendPhotoAsync(chatId: chatId, photo: cardUri,
                            cancellationToken: cancellationToken);
                    }
                    catch (NoCardFoundException)
                    {
                        Console.WriteLine($"No card image found for: {cardNames[0]}");
                    }
                }
                else if (cardNames.Count > 1)
                {
                    var inputMedia = new List<IAlbumInputMedia>();
                    foreach (var cardName in cardNames)
                    {
                        try
                        {
                            var cardUri = await _scryfallApi.GetCardImageUrlByName(cardName);
                            inputMedia.Add(new InputMediaPhoto(cardUri));
                            //following scryfall api's guidelines
                            await Task.Delay(55, cancellationToken);
                        }
                        catch (NoCardFoundException)
                        {
                            Console.WriteLine($"No card image found for: {cardName}");
                        }
                    }

                    for (var i = 0; i < inputMedia.Count; i = i + 6)
                    {
                        List<IAlbumInputMedia> mediaSubBatch;

                        mediaSubBatch = inputMedia.Count - 6 < i ? inputMedia.Skip(i).ToList() : inputMedia.GetRange(i, 6);

                        Console.WriteLine($"Sending photo for card names - {string.Join(", ", cardNames)}.");
                        await botClient.SendMediaGroupAsync(chatId, mediaSubBatch, cancellationToken: cancellationToken);
                    }
                }
            }
        }
    }
}