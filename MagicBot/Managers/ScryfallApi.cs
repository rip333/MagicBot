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
        private HttpClient _httpClient;
        public ScryfallApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCardImageUrlByName(string name)
        {
            string jsonString;
            if (name.ToUpper().StartsWith("RANDOM"))
            {
                var query = HttpUtility.HtmlDecode(name.ToUpper().Remove(0, 6).Trim());
                var response = await _httpClient.GetAsync($"cards/random?q={query}");
                jsonString = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var response = await _httpClient.GetAsync("cards/named?fuzzy=" + name);
                jsonString = await response.Content.ReadAsStringAsync();
            }

            var card = JsonConvert.DeserializeObject<Card>(jsonString);
            if (card.Status == 404 || card.ImageUris.Normal == null)
            {
                throw new NoCardFoundException();
            }

            return card.ImageUris.Normal;
        }
    }
}