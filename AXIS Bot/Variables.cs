using System.Collections.Generic;
using Discord.WebSocket;

namespace AXIS_Bot
{
    public class Variables
    {
        public static DiscordSocketClient Client;
        public static List<JoinLog> logList = new List<JoinLog>();
    }
}
