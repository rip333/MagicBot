using System.Threading.Tasks;
using MagicBot.Models.Scryfall;

namespace MagicBot.Managers
{
    public interface IScryfallApi
    {
        Task<Card> GetCardByName(string name);
        Task<Card> GetCardBySetCodeAndCollectorNumber(GetCardImageUriBySetCodeAndCollectorNumberRequest request);
    }
}