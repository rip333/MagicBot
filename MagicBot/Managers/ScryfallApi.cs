using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using MagicBot.Models.Exceptions;
using MagicBot.Models.Scryfall;
using Newtonsoft.Json;

namespace MagicBot.Managers
{
    public class ScryfallApi : IScryfallApi
    {
        private IScryfallClient _scryfallClient;
        public ScryfallApi(IScryfallClient scryfallClient)
        {
            _scryfallClient = scryfallClient;
        }

        public async Task<Card> GetCardByName(string name)
        {
            string jsonString;
            if (name.ToUpper().StartsWith("RANDOM"))
            {
                var query = HttpUtility.HtmlDecode(name.ToUpper().Remove(0, 6).Trim());
                jsonString = await _scryfallClient.GetRandomCardWithQuery(query);
            }
            else
            {
                jsonString = await _scryfallClient.GetFuzzyNamedCard(name);
            }

            var card = JsonConvert.DeserializeObject<Card>(jsonString);
            if (card.Status == 404)
            {
                throw new NoCardFoundException();
            }

            return card;
        }

        public async Task<Card> GetCardBySetCodeAndCollectorNumber(GetCardImageUriBySetCodeAndCollectorNumberRequest request)
        {
            var jsonString = await _scryfallClient.GetCardBySetCodeAndCollectorNumber(request.SetCode, request.CollectorNumber);
            var card = JsonConvert.DeserializeObject<Card>(jsonString);
            if (card.Status == 404)
            {
                throw new NoCardFoundException();
            }
            return card;
        }
    }
}