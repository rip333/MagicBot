using System.Threading.Tasks;

namespace MagicBot.Managers
{
    public interface IScryfallClient
    {
        Task<string> GetFuzzyNamedCard(string name);
        Task<string> GetRandomCardWithQuery(string query = "");
        Task<string> GetCardBySetCodeAndCollectorNumber(string setCode, string collectorNumber);
    }
}