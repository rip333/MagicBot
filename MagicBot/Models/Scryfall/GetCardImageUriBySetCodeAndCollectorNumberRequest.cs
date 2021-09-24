namespace MagicBot.Models.Scryfall
{
    public class GetCardImageUriBySetCodeAndCollectorNumberRequest
    {
        public GetCardImageUriBySetCodeAndCollectorNumberRequest(string setCode, string collectorNumber)
        {
            SetCode = setCode;
            CollectorNumber = collectorNumber;
        }
        
        public string SetCode { get; set; }
        public string CollectorNumber { get; set; }
    }
}