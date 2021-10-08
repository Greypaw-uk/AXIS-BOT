using System;
using System.Collections.Generic;
using System.IO;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace AXIS_Bot
{
    public class Settings
    {
        public int ProbationPeriod { get; set; }
        public int GCWTimer { get; set; }
        public string GuildID { get; set; }
        public string ChannelID { get; set; }
    }

    public static class AppSettings
    {
        public static DiscordSocketClient Client;
        public static List<JoinLog> logList = new List<JoinLog>();

        //Probation
        public static int ProbationDays = 14;

        //GCW
        public static int TimeOffset = 10;

        // Channel Tokens
        public static ulong GuildID;
        public static ulong ChannelID;


        public static void LoadSettings()
        {
            if (File.Exists("Settings.json"))
            {
                Console.WriteLine("Settings file located");

                Settings settings;

                //Deserialize existing json from log
                using StreamReader reader = new StreamReader("Settings.json");
                {
                    var json = reader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(json);
                }

                ProbationDays = settings.ProbationPeriod;
                TimeOffset = settings.GCWTimer;
                GuildID = Convert.ToUInt64(settings.GuildID);
                ChannelID = Convert.ToUInt64(settings.ChannelID);

                Console.WriteLine("Settings loaded");
            }
            else
            {
                Console.WriteLine("Settings file not found");
            }
        }

        public static void WriteSettings()
        {
            Settings settings = new Settings
            {
                ProbationPeriod = ProbationDays,
                GCWTimer = TimeOffset,
                GuildID = GuildID.ToString(),
                ChannelID = ChannelID.ToString()
            };

            var _settings = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText("Settings.json", _settings);
        }

        public static void LoadUserLogs()
        {
            if (File.Exists("UserLog.json"))
            {
                Console.WriteLine("Userlog located");
                try
                {
                    //Deserialize existing json from log
                    using StreamReader reader = new StreamReader("UserLog.json");
                    {
                        var json = reader.ReadToEnd();
                        logList = JsonConvert.DeserializeObject<List<JoinLog>>(json);
                    }

                    Console.WriteLine("Userlog loaded");
                }
                catch (Exception)
                {
                    Console.WriteLine("Unable to deserialize Users");
                }
            }
            else
            {
                Console.WriteLine("Userlog not found");
            }
        }
    }
}
