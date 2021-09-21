namespace MagicBot.Managers
{
    public static class TokenManager
    {
        private const string TokenLocation = @"E:\Dev\TelegramBots\MagicBot\token.txt";
        
        public static string Get()
        {
            return System.IO.File.ReadAllText(TokenLocation);
        }
    }
}