using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBot.Models.Enums;
using MagicBot.Models.Exceptions;
using MagicBot.Models.Scryfall;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MagicBot.Managers
{
    public class UpdateManager
    {
        private IScryfallApi _scryfallApi;

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

            var text = update.Message.Text;
            if (ChatUtility.ShouldProcessMessage(update.Message))
            {
                var messageType = ChatUtility.ProcessMessageType(text);
                switch (messageType)
                {
                    case MagicMessageType.Card:
                    {
                        var cardNames = ChatUtility.GetCardNamesFromBracketedMessage(text);
                        var cards = await GetCardsFromNames(cardNames);
                        var cardUris = GetImageUrisFromCardList(cards);
                        await SendCardImageMessages(botClient, cancellationToken, cardUris, chatId);
                        break;
                    }
                    case MagicMessageType.Deck:
                    {
                        var mainDeck = ChatUtility.GetMainDeckFromDeckList(text);
                        var cardRequests = ChatUtility.GetRequestsFromDecklist(mainDeck);
                        var cards = await GetCardsFromSetCodeAndCollectorNumberRequests(cardRequests);
                        var cardUris = GetImageUrisFromCardList(cards);
                        await SendCardImageMessages(botClient, cancellationToken, cardUris, chatId);

                        //Sideboard
                        if (ChatUtility.HasSideboard(text))
                        {
                            await botClient.SendTextMessageAsync(chatId, "-----Sideboard-----", cancellationToken: cancellationToken);
                            var sideboard = ChatUtility.GetSideboardFromDecklist(text);
                            var sideboardCardRequests = ChatUtility.GetRequestsFromDecklist(sideboard);
                            var sideBoardCards = await GetCardsFromSetCodeAndCollectorNumberRequests(sideboardCardRequests);
                            var sideboardCardUris = FilterBasicLands(sideBoardCards).Where(x => x.ImageUris is {Normal: { }}).Select(x => x.ImageUris.Normal).ToList();
                            await SendCardImageMessages(botClient, cancellationToken, sideboardCardUris, chatId);
                        }
                        break;
                    }
                }
            }
            
        }

        private static List<string> GetImageUrisFromCardList(List<Card> cards)
        {
            var filteredCardList = FilterBasicLands(cards);
            var cardList = filteredCardList as Card[] ?? filteredCardList.ToArray();
            var uris = cardList.Where(x => x.ImageUris is {Normal: { }}).Select(x => x.ImageUris.Normal).ToList();
            foreach (var card in cardList)
            {
                if (card.CardFaces == null) continue;
                foreach (var cardFace in card.CardFaces)
                {
                    if (!string.IsNullOrEmpty(cardFace.ImageUris.Normal))
                    {
                        uris.Add(cardFace.ImageUris.Normal);
                    }
                }
            }

            return uris;
        }

        private static IEnumerable<Card> FilterBasicLands(List<Card> cardList)
        {
            return cardList.Where(x => !x.TypeLine.Contains("Basic"));
        }

        private async Task<List<Card>> GetCardsFromNames(List<string> names)
        {
            var cardList = new List<Card>();
            foreach (var name in names)
            {
                try
                { 
                    var card = await _scryfallApi.GetCardByName(name);
                    cardList.Add(card);
                    Console.WriteLine($"Successfully acquired image uri for name: {name}");
                }
                catch (NoCardFoundException)
                {
                    Console.WriteLine($"No card image found for: {name}");
                }
            }
            return cardList;
        }

        private async Task<List<Card>> GetCardsFromSetCodeAndCollectorNumberRequests(
            List<GetCardImageUriBySetCodeAndCollectorNumberRequest> requests)
        {
            var cardList = new List<Card>();
            foreach (var request in requests)
            {
                try {
                    var card = await _scryfallApi.GetCardBySetCodeAndCollectorNumber(request);
                    cardList.Add(card);
                    Console.WriteLine($"Successfully acquired image uri for setCode/collectorNumber: {request.SetCode}/{request.CollectorNumber}");
                }
                catch (NoCardFoundException)
                {
                    Console.WriteLine($"No card image found for setCode/collectorNumber: {request.SetCode}/{request.CollectorNumber}");
                }
            }
            return cardList;
        }

        private async Task SendCardImageMessages(ITelegramBotClient botClient, CancellationToken cancellationToken,
            List<string> cardUris, long chatId)
        {
            if (cardUris.Count == 1)
            {
                await botClient.SendPhotoAsync(chatId: chatId, photo: cardUris[0],
                        cancellationToken: cancellationToken);
            }
            else if (cardUris.Count > 1)
            {
                var inputMedia = new List<IAlbumInputMedia>();
                foreach (var cardUri in cardUris)
                {

                        inputMedia.Add(new InputMediaPhoto(cardUri));
                }

                for (var i = 0; i < inputMedia.Count; i = i + 6)
                {
                    List<IAlbumInputMedia> mediaSubBatch;

                    mediaSubBatch = inputMedia.Count - 6 < i ? inputMedia.Skip(i).ToList() : inputMedia.GetRange(i, 6);

                    var retry = 0;

                    while (retry < 3)
                    {
                        try
                        {
                            await botClient.SendMediaGroupAsync(chatId, mediaSubBatch,
                                cancellationToken: cancellationToken);
                            retry = 3;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            retry++;
                            if (retry < 3)
                            {
                                Console.WriteLine($"Retry #{retry}");
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Finished processing chat {chatId}");
        }
    }
}