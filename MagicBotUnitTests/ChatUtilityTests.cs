using System;
using System.Collections.Generic;
using System.Linq;
using MagicBot.Managers;
using MagicBot.Models.Enums;
using MagicBot.Models.Scryfall;
using MagicBotUnitTests.TestUtilities;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace MagicBotUnitTests
{
    public class ChatManagerTests
    {
        
        [TestCase("[[cardname]]")]
        [TestCase("[[card     name]]")]
        [TestCase("I'm running [[cardname]]")]
        [TestCase("MY deck contains [[cardname1]] and [[cardname2]]")]
        public void ShouldReturnImageUri_True(string text)
        {
            Assert.True(ChatUtility.ShouldProcessMessage(new Message(){Text = text, Date = DateTime.UtcNow}));
        }
        
        [TestCase("")]
        [TestCase("[cardname]")]
        [TestCase("[[cardname")]
        [TestCase("]] nonsense [[")]
        public void ShouldReturnImageUri_FalseText(string text)
        {
            Assert.False(ChatUtility.ShouldProcessMessage(new Message(){Text = text, Date = DateTime.UtcNow}));
        }

        [Test]
        public void ShouldReturnImageUri_FalseDate()
        {
            Assert.False(ChatUtility.ShouldProcessMessage(new Message(){ Date = DateTime.UtcNow.AddMinutes(-6)}));
        }

        [TestCase("[[card1]]", new [] { "card1"})]
        [TestCase("[[card1]] and [[card2]]", new [] { "card1", "card2"})]
        [TestCase("[[card1]] and [[card1]]", new [] { "card1"})]
        [TestCase("[[card1 with a long name]] and [[card2]] and [[c]] text text text [[", 
            new [] { "card1 with a long name", "card2", "c"})]
        public void GetCardNamesInMessage_TestCase(string message, string[] cardNames)
        {
            Assert.AreEqual(cardNames.ToList(), ChatUtility.GetCardNamesFromBracketedMessage(message));
        }

        [TestCase("4 Champion of the Perished (MID) 91", "MID")]
        [TestCase("2 Draugr Necromancer (KHM) 86", "KHM")]
        [TestCase("4 Death-Priest of Myrkul (AFR) 95", "AFR")]
        public void GetSetCodeFromCardLine_TestCase(string deckListLine, string expected)
        {
            Assert.AreEqual(expected, ChatUtility.GetSetCodeFromCardLine(deckListLine));
        }
        
        [TestCase("4 Champion of the Perished (MID) 91", "91")]
        [TestCase("2 Draugr Necromancer (KHM) 86", "86")]
        [TestCase("4 Death-Priest of Myrkul (AFR) 95", "95")]
        public void GetCollectorNumberFromCardLine_TestCase(string deckListLine, string expected)
        {
            Assert.AreEqual(expected, ChatUtility.GetCollectorNumberFromCardLine(deckListLine));
        }

        [TestCase("[[card]]")]
        [TestCase("[[card1]] and [[card2]]")]
        [TestCase("My favorite card is [[card]]")]
        public void ProcessMessageType_Card(string message)
        {
            Assert.AreEqual(MagicMessageType.Card, ChatUtility.ProcessMessageType(message));
        }
        
        [Test]
        public void ProcessMessageType_DeckWithoutSideboard()
        {
            Assert.AreEqual(MagicMessageType.Deck, ChatUtility.ProcessMessageType(TestData.DeckWithoutSideboard));
        }
        
        [Test]
        public void ProcessMessageType_DeckWithSideboard()
        {
            Assert.AreEqual(MagicMessageType.Deck, ChatUtility.ProcessMessageType(TestData.DeckWithSideboard));
        }
        
        [TestCase("this is not a deck or a card message")]
        public void ProcessMessageType_Error(string message)
        {
            Assert.AreEqual(MagicMessageType.Error, ChatUtility.ProcessMessageType(message));
        }

        [Test]
        public void GetMainDeckFromDeckList_WithSideboard()
        {
            Assert.AreEqual("5 Mountain (MID) 383\n ", ChatUtility.GetMainDeckFromDeckList(TestData.DeckWithSideboard));
        }
        
        [Test]
        public void GetMainDeckFromDeckList_WithoutSideboard()
        {
            Assert.AreEqual("5 Mountain (MID) 383", ChatUtility.GetMainDeckFromDeckList(TestData.DeckWithoutSideboard));
        }

        [Test]
        public void HasSideboard_True()
        {
            Assert.True(ChatUtility.HasSideboard(TestData.DeckWithSideboard));
        }
        
        [Test]
        public void HasSideboard_False()
        {
            Assert.False(ChatUtility.HasSideboard(TestData.DeckWithoutSideboard));
        }

        [Test]
        public void GetSideboardFromDecklist_TestCase()
        {
            Assert.AreEqual(" 2 Illuminate History (STX) 108", ChatUtility.GetSideboardFromDecklist(TestData.DeckWithSideboard));
        }

        [Test]
        public void GetRequestsFromDecklist_TestCase()
        {
            var expectedRequests = new List<GetCardImageUriBySetCodeAndCollectorNumberRequest>()
            {
                new ("MID", "383"),
                new ("MID", "384"),
                new ("RIP", "123")
            };
            var actual = ChatUtility.GetRequestsFromDecklist(TestData.TrimmedDecklist);
            Assert.AreEqual(expectedRequests[0].CollectorNumber, actual[0].CollectorNumber);
            Assert.AreEqual(expectedRequests[0].SetCode, actual[0].SetCode);

            Assert.AreEqual(expectedRequests[1].CollectorNumber, actual[1].CollectorNumber);
            Assert.AreEqual(expectedRequests[1].SetCode, actual[1].SetCode);
            
            Assert.AreEqual(expectedRequests[2].CollectorNumber, actual[2].CollectorNumber);
            Assert.AreEqual(expectedRequests[2].SetCode, actual[2].SetCode);
        }
    }
}