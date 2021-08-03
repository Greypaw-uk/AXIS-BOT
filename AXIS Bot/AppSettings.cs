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
    }

    public static class AppSettings
    {
        public static DiscordSocketClient Client;
        public static List<JoinLog> logList = new List<JoinLog>();

        //Probation
        public static int ProbationDays = 14;

        //GCW
        public static int TimeOffset = 10;


        public static void LoadSettings()
        {
            if (File.Exists("Settings.json"))
            {
                Console.WriteLine("Settings file located");

                var settings = new Settings();

                //Deserialize existing json from log
                using StreamReader reader = new StreamReader("Settings.json");
                {
                    var json = reader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(json);
                }

                AppSettings.ProbationDays = settings.ProbationPeriod;
                TimeOffset = settings.GCWTimer;
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
                ProbationPeriod = AppSettings.ProbationDays,
                GCWTimer = TimeOffset
            };

            string _settings = JsonConvert.SerializeObject(settings, Formatting.Indented);
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
