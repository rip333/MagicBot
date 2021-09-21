using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace MagicBot.Managers
{
    public class ChatUtility
    {
        public static bool ShouldReturnImageUri(Message message)
        {
            if (message.Date.AddMinutes(5) < DateTime.UtcNow)
            {
                return false;
            }
            if (message.Text.Contains("[[") && message.Text.Contains("]]"))
            {
                return message.Text.IndexOf("[[", StringComparison.Ordinal) <
                       message.Text.IndexOf("]]", StringComparison.Ordinal);
            }

            return false;
        }

        public static List<string> GetCardNamesInMessage(string message)
        {
            var splitString = message.Split("[[");

            return (from str in splitString where str.Contains("]]") select str.Split("]]")[0]).Distinct().ToList();
        }
    }
}