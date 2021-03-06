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
        [JsonProperty("set")]
        public string Set { get; set; }
        [JsonProperty("collector_number")]
        public string CollectorNumber { get; set; }
        [JsonProperty("type_line")]
        public string TypeLine { get; set; }
        
        [JsonProperty("card_faces")]
        public CardFace[] CardFaces { get; set; }

    }

    public class ImageUris
    {
        [JsonProperty("normal")]
        public string Normal { get; set; }
    }

    public class CardFace
    {
        [JsonProperty("image_uris")]
        public ImageUris ImageUris { get; set; }
    }
}