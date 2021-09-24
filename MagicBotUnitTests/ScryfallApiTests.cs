using System.Threading.Tasks;
using MagicBot.Managers;
using MagicBot.Models.Exceptions;
using MagicBot.Models.Scryfall;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MagicBotUnitTests
{
    public class ScryfallApiTests
    {
        private IScryfallApi _scryfallApi;
        private Card card;
        private Mock<IScryfallClient> _mockScryfallClient;

        [SetUp]
        public void TestSetup()
        {
            _mockScryfallClient = new Mock<IScryfallClient>();
            _scryfallApi = new ScryfallApi(_mockScryfallClient.Object);
        }
        
        [Test]
        public async Task GetCardImageUrlByName_RandomCard()
        {
            var randomCard = new Card()
            {
                Name = "Random Card",
                ImageUris = new ImageUris()
                {
                    Normal = "Random Card URI"
                },
                Status = 200
            };

            var jsonCard = JsonConvert.SerializeObject(randomCard);
            _mockScryfallClient.Setup(x => x.GetRandomCardWithQuery(It.Is<string>(y => y == ""))).ReturnsAsync(jsonCard);

            var response = await _scryfallApi.GetCardByName("random");
            Assert.AreEqual(randomCard.ImageUris.Normal, response.ImageUris.Normal);
        }
        
        [Test]
        public async Task GetCardImageUrlByName_RandomCard_WithQuery()
        {
            var randomCard = new Card()
            {
                Name = "Random Card",
                ImageUris = new ImageUris()
                {
                    Normal = "Random Card with query URI"
                },
                Status = 200
            };

            var jsonCard = JsonConvert.SerializeObject(randomCard);
            _mockScryfallClient.Setup(x => x.GetRandomCardWithQuery(It.Is<string>(y => y == "QUERY"))).ReturnsAsync(jsonCard);

            var response = await _scryfallApi.GetCardByName("random query");
            Assert.AreEqual(randomCard.ImageUris.Normal, response.ImageUris.Normal);
        }
        
        [Test]
        public async Task GetCardImageUrlByName_NamedCard()
        {
            var namedCard = new Card()
            {
                Name = "Named Card",
                ImageUris = new ImageUris()
                {
                    Normal = "Named URI"
                },
                Status = 200
            };

            const string nameQuery = "NAME_QUERY";
            var jsonCard = JsonConvert.SerializeObject(namedCard);
            _mockScryfallClient.Setup(x => x.GetFuzzyNamedCard(It.Is<string>(y => y == nameQuery))).ReturnsAsync(jsonCard);

            var response = await _scryfallApi.GetCardByName(nameQuery);
            Assert.AreEqual(namedCard.ImageUris.Normal, response.ImageUris.Normal);
        }
        
        [Test]
        public void GetCardImageUrlByName_ErrorStatus()
        {
            var errorCard = new Card()
            {
                Status = 404
            };

            const string errorQuery = "ERROR_QUERY";
            var jsonCard = JsonConvert.SerializeObject(errorCard);
            _mockScryfallClient.Setup(x => x.GetFuzzyNamedCard(It.Is<string>(y => y == errorQuery))).ReturnsAsync(jsonCard);

            Assert.ThrowsAsync<NoCardFoundException>(() => _scryfallApi.GetCardByName(errorQuery));
        }
    }
}