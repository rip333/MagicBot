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
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        public async Task<string> GetRandomCardWithQuery(string query = "")
        {
            var response = await _httpClient.GetAsync($"cards/random?q={query}");
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }
    }
}