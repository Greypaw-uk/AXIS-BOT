using Newtonsoft.Json.Linq;

namespace AXIS_Bot
{
    static class API
    {
        private static string previousStatus;

        private static string SWGURL = "https://swglegends.com/server_status_test.php";
        public static string SWGStatus()
        {
            var status = string.Empty;

            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(SWGURL);
                var _json = JObject.Parse(json);

                foreach (var result in _json["stats"]["Omega"]["most_recent"])
                    if (result.ToString().Contains("status"))
                        status = result.ToString();

                status = status.Replace("status", "").Replace("\\", "").Replace(":", "").Replace("\"", "")
                    .Replace(" ", "");

                return status.ToLower();
            }
        }
    }
}
