using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicBot.Managers
{
    public class ChatUtility
    {
        public static bool ShouldReturnImageUri(string message)
        {
            if (message.Contains("[[") && message.Contains("]]"))
            {
                return message.IndexOf("[[", StringComparison.Ordinal) <
                       message.IndexOf("]]", StringComparison.Ordinal);
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