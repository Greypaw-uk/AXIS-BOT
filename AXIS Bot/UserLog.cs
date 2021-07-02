using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    static class UserLog
    {
        public static bool CreateUserLog(SocketGuildUser gUser)
        {
            if (gUser.IsBot || gUser.IsWebhook) return false;

            try
            {
                //Re-instantiate list if deleted/null
                Variables.logList ??= new List<JoinLog>();

                //Create new log entry and add to logList
                var log = new JoinLog
                {
                    User = gUser.Id,
                    Name = gUser.Username,
                    Date = DateTime.Today.ToShortDateString()
                };
                Variables.logList.Add(log);


                //Serialize logList to UserLog.json
                string logs = JsonConvert.SerializeObject(Variables.logList, Formatting.Indented);
                File.WriteAllText("UserLog.json", logs);

                Console.WriteLine("User \'" + Variables.Client.GetUser(log.User) + "\' has joined the channel.");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        public static string GetUserJoinedDetails(string chat)
        {
            var result = "";

            var user = new string(chat.Where(char.IsDigit).ToArray());
            var userID = Convert.ToUInt64(user);

            foreach (var log in Variables.logList.Where(log => log.User == userID))
            {
                var futureDate = DateTime.Parse(log.Date).AddDays(14); 
                
                result = log.Name + " joined " + 
                         DateTime.Parse(log.Date).ToLongDateString() + ".  Their probation is due " + futureDate.ToLongDateString();
            }

            return result;
        }
    }
}
