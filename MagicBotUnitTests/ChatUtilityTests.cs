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
            Assert.True(ChatUtility.ShouldReturnImageUri(new Message(){Text = text, Date = DateTime.UtcNow}));
        }
        
        [TestCase("")]
        [TestCase("[cardname]")]
        [TestCase("[[cardname")]
        [TestCase("]] nonsense [[")]
        public void ShouldReturnImageUri_FalseText(string text)
        {
            Assert.False(ChatUtility.ShouldReturnImageUri(new Message(){Text = text, Date = DateTime.UtcNow}));
        }

        [Test]
        public void ShouldReturnImageUri_FalseDate()
        {
            Assert.False(ChatUtility.ShouldReturnImageUri(new Message(){ Date = DateTime.UtcNow.AddMinutes(-6)}));
        }

        [TestCase("[[card1]]", new [] { "card1"})]
        [TestCase("[[card1]] and [[card2]]", new [] { "card1", "card2"})]
        [TestCase("[[card1]] and [[card1]]", new [] { "card1"})]
        [TestCase("[[card1 with a long name]] and [[card2]] and [[c]] text text text [[", 
            new [] { "card1 with a long name", "card2", "c"})]
        public void GetCardNamesInMessage_TestCase(string message, string[] cardNames)
        {
            Assert.AreEqual(cardNames.ToList(), ChatUtility.GetCardNamesInMessage(message));
        }
    }
}