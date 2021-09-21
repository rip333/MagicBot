using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
            var response = await _httpClient.GetAsync("cards/named?fuzzy=" + name);
            var jsonString = await response.Content.ReadAsStringAsync();
            var card = JsonConvert.DeserializeObject<Card>(jsonString);
            if (card.Status == 404)
            {
                throw new NoCardFoundException();
            }
            return card.ImageUris.Normal;
        }
    }
}