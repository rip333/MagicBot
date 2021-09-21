using System.Threading.Tasks;

namespace MagicBot.Managers
{
    public interface IScryfallApi
    {
        Task<string> GetCardImageUrlByName(string name);
    }
}