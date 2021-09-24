using System;
using System.Collections.Generic;
using System.Linq;
using MagicBot.Models.Enums;
using MagicBot.Models.Scryfall;
using Telegram.Bot.Types;

namespace MagicBot.Managers
{
    public class ChatUtility
    {
        public static MagicMessageType ProcessMessageType(string message)
        {
            if(DoesMessageContainBracketedNames(message)) 
                return MagicMessageType.Card;
            if (IsMessageDecklist(message))
                return MagicMessageType.Deck;
            return MagicMessageType.Error;
        }

        public static string GetMainDeckFromDeckList(string deckList)
        {
            var mainDeck = deckList.Split("Sideboard\n")[0];
            return mainDeck.Remove(0, 6);
        }

        public static bool HasSideboard(string deckList)
        {
            return deckList.Contains("Sideboard\n");
        }

        public static string GetSideboardFromDecklist(string deckList)
        {
            return deckList.Split("Sideboard\n")[1];
        }

        public static List<GetCardImageUriBySetCodeAndCollectorNumberRequest> GetRequestsFromDecklist(string deckList)
        {
            var cardRequests = new List<GetCardImageUriBySetCodeAndCollectorNumberRequest>();

            var cardLines = deckList.Split("\n");

            foreach (var cardLine in cardLines)
            {
                if (string.IsNullOrEmpty(cardLine)) continue;
                var setCode = GetSetCodeFromCardLine(cardLine);
                var collectorNumber = GetCollectorNumberFromCardLine(cardLine);
                cardRequests.Add(new GetCardImageUriBySetCodeAndCollectorNumberRequest(setCode, collectorNumber));
            }
            
            return cardRequests;
        }

        public static string GetSetCodeFromCardLine(string deckListLine)
        {
            var cardParts = deckListLine.Split('(');
            return (from str in cardParts where str.Contains(")") select str.Split(")")[0]).First();
        }

        public static string GetCollectorNumberFromCardLine(string deckListLine)
        {
            var cardParts = deckListLine.Split(' ');
            return cardParts[^1];
        }

        public static List<string> GetCardNamesFromBracketedMessage(string message)
        {
            var splitString = message.Split("[[");

            return (from str in splitString where str.Contains("]]") select str.Split("]]")[0]).Distinct().ToList();
        }

        public static bool ShouldProcessMessage(Message message)
        {
            if (message.Date.AddMinutes(5) < DateTime.UtcNow)
            {
                return false;
            }
            if (DoesMessageContainBracketedNames(message.Text))
            {
                return true;
            }
            if (IsMessageDecklist(message.Text))
            {
                return true;
            }

            return false;
        }

        public static bool IsMessageDecklist(string message)
        {
            return message.StartsWith("Deck\n") && !message.Contains("[[") && !message.Contains("]]") 
                   && message.Contains('(') && message.Contains(')');
        }

        public static bool DoesMessageContainBracketedNames(string message)
        {
            return message.Contains("[[") && message.Contains("]]") &&
                   message.IndexOf("[[", StringComparison.Ordinal) <
                   message.IndexOf("]]", StringComparison.Ordinal);
        }
    }
}