using System.Net;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace AXIS_Bot
{
    public class ServerMonitor
    {
        public static bool isServerMonitorOn = false;

        public static async Task LoopServerStatus(ISocketMessageChannel channel)
        {
            var isMessageSent = false;
            var previousStatus = string.Empty;
            isServerMonitorOn = true;

            await channel.SendMessageAsync("Running server monitor...");

            if (string.IsNullOrEmpty(previousStatus))
                previousStatus = SWGStatus();

            while (isServerMonitorOn)
            {
                var now = SystemClock.Instance.GetCurrentInstant();
                var secs = now.InUtc().Second;

                if (!isMessageSent && secs < 2)
                {
                    var currentStatus = SWGStatus();

                    if (!currentStatus.Equals(previousStatus))
                    {
                        if (currentStatus.ToLower().Equals("offline"))
                        {
                            await channel.SendMessageAsync("Server Offline.");
                            isMessageSent = true;
                        }
                        else if (currentStatus.ToLower().Equals("loading"))
                        {
                            await channel.SendMessageAsync("Server Loading.");
                            isMessageSent = true;
                        }
                        else if (currentStatus.ToLower().Equals("unknown"))
                        {
                            await channel.SendMessageAsync("Server status unknown.");
                            isMessageSent = true;
                        }
                        else
                        {
                            await channel.SendMessageAsync("Server Online.");
                            isMessageSent = true;
                        }
                    }

                    previousStatus = currentStatus;

                    //Throttle the bot for a couple of seconds to stop multiple messages going out on faster CPUs
                    if (isMessageSent)
                    {
                        await Task.Delay(2000);
                        isMessageSent = false;
                    }

                    await Task.Delay(60000);
                }
            }
        }

        public static string SWGStatus()
        {
            using var webClient = new WebClient();

            //Parse API to JSON
            var SWGURL = "https://swglegends.com/server_status_test.php";
            var json = webClient.DownloadString(SWGURL);
            var _json = JObject.Parse(json);

            //Check for null as there is a case where the JSON provided by Legends appears malformed, and results in a NullReferenceException
            if (_json["stats"]["Omega"]["most_recent"] == null) return "unknown";

            //Extract server status from json string
            var status = string.Empty;
            foreach (var result in _json["stats"]["Omega"]["most_recent"])
                if (result.ToString().Contains("status"))
                    status = result.ToString();

            //Remove formatting around server status, leaving only the status as a word
            status = status.Replace("status", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("\"", "")
                .Replace(" ", "");

            return status.ToLower();
        }
    }
}
