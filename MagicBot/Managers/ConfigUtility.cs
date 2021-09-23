using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MagicBot.Managers
{
    public class ConfigUtility
    {
        public static async Task<string> GetToken()
        {
            //Setup Config and token
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();
            var tokenFileLocation = config["AppSettings:TokenFileLocation"];
            var token = await System.IO.File.ReadAllTextAsync(tokenFileLocation);
            return token;
        }
    }
}