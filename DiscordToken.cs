using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;

namespace DiscordBot
{
    internal class DiscordToken
    {
        private static string token = "";
        public static string Token
        {
            get { return token; }
        }
    }
}
