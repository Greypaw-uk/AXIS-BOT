using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace AXIS_Bot
{
    public class JoinLog
    {
        public ulong User { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }

    // hello
    static class Probation
    {
        public static bool CreateUserLog(SocketGuildUser gUser)
        {
            if (gUser.IsBot || gUser.IsWebhook) return false;

            try
            {
                //Re-instantiate list if deleted/null
                AppSettings.logList ??= new List<JoinLog>();

                //Create new log entry and add to logList
                var log = new JoinLog
                {
                    User = gUser.Id,
                    Name = gUser.Username,
                    Date = DateTime.Today.ToShortDateString()
                };
                AppSettings.logList.Add(log);


                //Serialize logList to UserLog.json
                string logs = JsonConvert.SerializeObject(AppSettings.logList, Formatting.Indented);
                File.WriteAllText("UserLog.json", logs);

                Console.WriteLine(DateTime.Now + ": " + "User \'" + AppSettings.Client.GetUser(log.User) + "\' has joined the channel.");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        //Gets user's details from !prob command and calculates when probation is due
        public static string GetUserJoinedDetails(string chat)
        {
            var result = "";

            //Extract userID from chat, and convert to uLong
            var user = new string(chat.Where(char.IsDigit).ToArray());
            var userID = Convert.ToUInt64(user);

            foreach (var log in AppSettings.logList.Where(log => log.User == userID))
            {
                var futureDate = DateTime.Parse(log.Date).AddDays(AppSettings.ProbationDays); 
                
                result = log.Name + " joined " + 
                         DateTime.Parse(log.Date).ToLongDateString() + ".  Their probation is due " + futureDate.ToLongDateString();
            }

            return result;
        }

        //Sets the number of days the probation period lasts for 
        public static string SetProbationPeriod(string chat)
        {
            var getDigits = new string(chat.Where(char.IsDigit).ToArray());
            AppSettings.ProbationDays = int.Parse(getDigits);

            AppSettings.WriteSettings();

            return "Probation period set to " + AppSettings.ProbationDays + " days.";
        }

        public static async void StartProbationLoop()
        {
            //Program.SendMessageToChannel("Probation loop started");

            var now = DateTime.Now;
            var tomorrow = now.AddDays(1);
            var durationUntilMidnight = tomorrow.Date - now;

            var t = new Timer(o =>
            {
                foreach (var log in AppSettings.logList)
                {
                    if (log.Date == DateTime.Today.ToShortDateString())
                    {
                        AppSettings.Client.GetGuild(808342010893303858).GetTextChannel(808342010893303861)
                            .SendMessageAsync(log.Name + " is due their probation today");
                    }
                }

            }, null, TimeSpan.Zero, durationUntilMidnight);
        }
    }
}
