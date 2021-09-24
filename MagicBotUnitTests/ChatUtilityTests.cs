using System;
using System.Linq;
using MagicBot.Managers;
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
    }
}