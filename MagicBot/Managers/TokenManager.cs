namespace MagicBot.Managers
{
    public static class TokenManager
    {
        //Replace this with a text file containing your telegram bot's token
        private const string TokenLocation = @"E:\Dev\TelegramBots\MagicBot\token.txt";
        
        public static string Get()
        {
            return System.IO.File.ReadAllText(TokenLocation);
        }
    }
}
