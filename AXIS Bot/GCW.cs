using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using NodaTime;

namespace AXIS_Bot
{
    public class GCWEvent
    {
        public string Location;
        public int Hour;
        public string Message;
        public string Coords;
    }

    public static class GCW
    {
        public static List<GCWEvent> eventsList = new List<GCWEvent>();

        public static bool IsLoopStarted;

        public static async Task LoopBarker(ISocketMessageChannel channel)
        {
            bool isMessageSent = false;
            IsLoopStarted = true;

            await channel.SendMessageAsync("Starting GCW Barker...");

            while (IsLoopStarted)
            {
                var now = SystemClock.Instance.GetCurrentInstant();
                var nowUTC = now.InUtc();

                var hours = nowUTC.Hour;
                var mins = nowUTC.Minute;
                var secs = nowUTC.Second;

                if (!isMessageSent && secs <= 1 && mins == (60 - AppSettings.TimeOffset))
                {
                    foreach (var entry in eventsList.Where(entry => entry.Hour == hours))
                    {
                        isMessageSent = true;
                        var outcome = BarkerMessage(entry);
                        Console.WriteLine(DateTime.Now + ": " + outcome);
                        await channel.SendMessageAsync(outcome);
                    }
                }

                //Throttle the bot for a couple of seconds to stop multiple messages going out on faster CPUs
                if (isMessageSent)
                {
                    await Task.Delay(2000);
                    isMessageSent = false;
                }

                await Task.Delay(1000);
            }
        }

        private static string BarkerMessage(GCWEvent entry)
        {
            var outcome = new StringBuilder();
            outcome.Append(entry.Location);
            outcome.Append(": ");
            outcome.Append(entry.Message);
            outcome.Append(" starts in ");
            outcome.Append(AppSettings.TimeOffset);
            outcome.Append(" minutes!");

            if (!string.IsNullOrEmpty(entry.Coords))
                outcome.Append(" Coords: " + entry.Coords);

            return outcome.ToString();
        }

        public static void PopulateEventList()
        {
            eventsList.Add(new GCWEvent
            {
                Location = "Naboo",
                Hour = 23,
                Message = "PVE Space battle",
                Coords = "/wp space_naboo -5032 -3958 4345 Space Battle Naboo;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dearic",
                Hour = 0,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Lok",
                Hour = 1,
                Message = "PVP space battle",
                Coords = "/wp space_lok 2933 473 -1450 Space Battle Lok;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Keren",
                Hour = 2,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Tatooine",
                Hour = 3,
                Message = "PVE Space battle",
                Coords = "/wp space_tatooine 4300 3900 1600 Space Battle Tatooine;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Bestine",
                Hour = 4,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dantooine",
                Hour = 5,
                Message = "PVP space battle",
                Coords = "/wp space_dantooine 2700 4400 -2800 Space Battle Dantooine;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dearic",
                Hour = 6,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Corellia",
                Hour = 7,
                Message = "PVE Space battle",
                Coords = "/wp space_corellia 1200 3000 5500 Space Battle Corellia;"
            });


            eventsList.Add(new GCWEvent
            {
                Location = "Keren",
                Hour = 8,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Lok",
                Hour = 9,
                Message = "PVP space battle",
                Coords = "/wp space_lok 2933 473 -1450 Space Battle Lok;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Bestine",
                Hour = 10,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Naboo",
                Hour = 11,
                Message = "PVE Space battle",
                Coords = "/wp space_naboo -5032 -3958 4345 Space Battle Naboo;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dearic",
                Hour = 12,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dantooine",
                Hour = 13,
                Message = "PVP space battle",
                Coords = "/wp space_dantooine 2700 4400 -2800 Space Battle Dantooine;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Keren",
                Hour = 14,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Corellia",
                Hour = 15,
                Message = "PVE Space battle",
                Coords = "/wp space_corellia 1200 3000 5500 Space Battle Corellia;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Bestine",
                Hour = 16,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Tatooine",
                Hour = 17,
                Message = "PVE Space battle",
                Coords = "/wp space_tatooine 4300 3900 1600 Space Battle Tatooine;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dearic",
                Hour = 18,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Lok",
                Hour = 19,
                Message = "PVP space battle",
                Coords = "/wp space_lok 2933 473 -1450 Space Battle Lok;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Keren",
                Hour = 20,
                Message = "Ground battle"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Dantooine",
                Hour = 21,
                Message = "PVP space battle",
                Coords = "/wp space_dantooine 2700 4400 -2800 Space Battle Dantooine;"
            });

            eventsList.Add(new GCWEvent
            {
                Location = "Bestine",
                Hour = 22,
                Message = "Ground battle"
            });
        }

        public static string GCWNext()
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var nowUTC = now.InUtc();
            var hour = nowUTC.Hour;
            var minutes = 60 - nowUTC.Minute;

            foreach (var entry in eventsList)
            {
                if (entry.Hour == hour)
                {
                    var sb = new StringBuilder();

                    sb.Append("Next event is at ");
                    sb.Append(entry.Location);
                    sb.Append(": ");
                    sb.Append(entry.Message);
                    sb.Append(" in " + minutes + " minutes");

                    if (!string.IsNullOrEmpty(entry.Coords))
                        sb.Append(" at " + entry.Coords);

                    return sb.ToString();
                }
            }

            return null;
        }

        public static int TrimTime(string time)
        {
            time = time.Replace("!settime ", "");
            time = time.TrimStart(new char[] {'0'});

            if (string.IsNullOrEmpty("time"))
                time = "0";

            var convertedTime = Convert.ToInt32(time);

            if (convertedTime > 59)
                convertedTime = 0;

            return convertedTime;
        }
    }
}
