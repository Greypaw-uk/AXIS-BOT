using Discord;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AXIS_Bot
{
	class Program
	{
		private DiscordSocketClient _client;
        private bool isServerMonitorOn = false;

        public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();
			_client.Log += Log;

			_client.MessageReceived += MessageReceived;

			// LIVE
			//await _client.LoginAsync(TokenType.Bot, "ODA3Mzk0NzI1MzkyMjg1NzE2.YB3W7w._ntuOOrB4TcmYUm1veb_3nexfaQ");
                
            // TEST
			await _client.LoginAsync(TokenType.Bot, "ODA4MDkxMTgwODUxMTM0NTI2.YCBfjw.5hdsbmh5NtNGf4vq3DJE7M-Eq5M");

			await _client.StartAsync();

			if (GCW.eventsList.Count == 0)
				GCW.PopulateEventList();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task MessageReceived(SocketMessage message)
		{
			string chat = message.Content.ToLower();

			if (chat.Equals("!gcwstart") && !chat.Equals("!about"))
				GCW.LoopBarker(message.Channel);

			if (chat.Equals("!gcwstop") && !chat.Equals("!about"))
			{
				GCW.IsLoopStarted = false;
				await message.Channel.SendMessageAsync("Stopping GCW Barker...");
			}

			if (chat.Contains("!settime") && !chat.Equals("!about"))
			{
				GCW.TimeOffset = GCW.TrimTime(chat);

				await message.Channel.SendMessageAsync("Alert time set to " + GCW.TimeOffset + " minutes.");
			}

			if (chat.Equals("!about"))
				await message.Channel.SendMessageAsync(Help());

			if (chat.Equals("!gcwnext") && !chat.Equals("!about"))
			{
				var next = GCW.GCWNext();

				if (!string.IsNullOrEmpty(next))
					await message.Channel.SendMessageAsync(GCW.GCWNext());
				else
					await message.Channel.SendMessageAsync("Unable to retrieve next battle");
			}

			if (chat.Equals("!status") && !chat.Equals("!about"))
				await message.Channel.SendMessageAsync(RexStatus());

			if (chat.Contains("!skills") && !chat.Equals("!about"))
			{
				Skills.ResetSkills();
				await message.Channel.SendMessageAsync(Skills.AllSkills(chat));
			}

            if (chat.Contains("!servermonitor") && !chat.Equals("!about"))
                LoopServerStatus(message.Channel);

            if (chat.Equals("!serverstatus") && !chat.Equals("!about"))
                await message.Channel.SendMessageAsync("The server is currently " + API.SWGStatus().ToLower());
        }

        private async Task LoopServerStatus(ISocketMessageChannel channel)
        {
            bool isMessageSent = false;
            string previousStatus = string.Empty;
            isServerMonitorOn = true;

            await channel.SendMessageAsync("Running server monitor...");

            if (string.IsNullOrEmpty(previousStatus))
                previousStatus = API.SWGStatus();

            while (isServerMonitorOn)
            {
                var now = SystemClock.Instance.GetCurrentInstant();
                var secs = now.InUtc().Second;

                if (!isMessageSent && secs < 2)
                {
                    var currentStatus = API.SWGStatus();

                    if (!currentStatus.Equals(previousStatus))
                    {
                        if (previousStatus.Equals("offline") || previousStatus.Equals("loading"))
                        {
                            if (currentStatus.Equals("online"))
                            {
                                await channel.SendMessageAsync("Server back online.");
                                isMessageSent = true;
                            }
						}
                        else if (previousStatus.Equals("online") || previousStatus.Equals("loading"))
                        {
                            if (currentStatus.Equals("offline"))
                            {
                                await channel.SendMessageAsync("Server has gone offline.");
                                isMessageSent = true;
                            }
						}
                    }

                    previousStatus = currentStatus;
				}

                //Throttle the bot for a couple of seconds to stop multiple messages going out on faster CPUs
                if (isMessageSent)
                {
                    await Task.Delay(2000);
                    isMessageSent = false;
                }

                await Task.Delay(60000);
            }
		}

		private string Help()
		{
			var help =
				"Command: !gcwstart \n" +
				"Result: begins the GCW Barker - announcements will be made in whichever channel the command was executed in \n\n" +
				
                "Command: !gcwstop \n" +
				"Result: stops the GCW Barker\n\n" +
				
                "Command: !settime \n" + 
				"Result: set the number of minutes prior to an event that the announcement is made, e.g. '!time 20' will set to 20 minutes.  Accepts 0-59. \n\n" +
				
                "Command: !gcwnext \n" +
				"Result: Lists the next GCW event to take place on the server. \n\n" +
				
                "Command: !status \n" +
				"Result: Returns bot's current settings. \n\n" +
				
                "Command: !skills [str] [con] [sta] [pre] [agi] [lck]\n" +
				"Results: Returns skill modifiers based on character's attributes. Attributes must be separated by a space e.g." +
						"'!skills 100 100 100 100 100 100'.\n\n" +
				
                "Command: !servermonitor \n" +
				"Result: Checks for changes to the SWG server status every 60 seconds and gives alerts when the status changes. \n\n" +
				
                "Command: !serverstatus \n" +
				"Result: Displays the current SWG server status. \n\n"+
                

                "Report bugs to bot's author, Ic'ma";

			return help;
		}

		private string RexStatus()
		{
			var sb = new StringBuilder();

            if (GCW.IsLoopStarted)
                sb.Append("GCW Barker is on. \n");
            else
                sb.Append("GCW Barker is off.\n");

			sb.Append("Alert time set to ");
			sb.Append(GCW.TimeOffset);
			sb.Append(" minutes.\n");

            if (isServerMonitorOn)
                sb.Append("Server monitor is on.");
            else
                sb.Append("Server monitor is off.");

			return sb.ToString();
		}
	}
}
