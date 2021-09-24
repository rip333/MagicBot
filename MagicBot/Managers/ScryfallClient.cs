using System.Net.Http;
using System.Threading.Tasks;

namespace MagicBot.Managers
{
    public class ScryfallClient : IScryfallClient
    {
        private HttpClient _httpClient;

        public ScryfallClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetFuzzyNamedCard(string name)
        {
            var response = await _httpClient.GetAsync("cards/named?fuzzy=" + name);
            //Throttling requests the easy way.  Scryfall api's rate limit guidelines - https://scryfall.com/docs/api
            await Task.Delay(55);
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        public async Task<string> GetRandomCardWithQuery(string query = "")
        {
            var response = await _httpClient.GetAsync($"cards/random?q={query}");
            await Task.Delay(55);
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        public async Task<string> GetCardBySetCodeAndCollectorNumber(string setCode, string collectorNumber)
        {
            var response = await _httpClient.GetAsync($"cards/{setCode.ToLower()}/{collectorNumber}");
            await Task.Delay(55);
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }
    }
}