using System.Collections.Generic;
using Newtonsoft.Json;

namespace MagicBot.Models.Scryfall
{
    [JsonArray]
    public class Cards { public List<Card> JSON; }
    
    public class Card
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("image_uris")]
        public ImageUris ImageUris { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class ImageUris
    {
        [JsonProperty("normal")]
        public string Normal { get; set; }
    }
}