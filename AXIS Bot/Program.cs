using Discord;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AXIS_Bot
{
	class Program
	{
        //Set environment to Live or Test
        private bool isLiveEnvironment = true;

        public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
            AppSettings.Client = new DiscordSocketClient();
            AppSettings.Client.Log += Log;
            AppSettings.Client.Ready += ClientReady;

            AppSettings.Client.MessageReceived += MessageReceived;

            AppSettings.Client.UserJoined += HandleUserJoinedAsync;

            // Pick between live or test tokens
            if (isLiveEnvironment)
			    await AppSettings.Client.LoginAsync(TokenType.Bot, "ODA3Mzk0NzI1MzkyMjg1NzE2.YB3W7w._ntuOOrB4TcmYUm1veb_3nexfaQ");
            else
			    await AppSettings.Client.LoginAsync(TokenType.Bot, "ODA4MDkxMTgwODUxMTM0NTI2.YCBfjw.5hdsbmh5NtNGf4vq3DJE7M-Eq5M");

			await AppSettings.Client.StartAsync();

			if (GCW.eventsList.Count == 0)
				GCW.PopulateEventList();

			// Block this task until the program is closed.
			await Task.Delay(-1);
        }

		//Create info of when a user joins the channel
        private Task HandleUserJoinedAsync(SocketGuildUser gUser)
        {
            return Task.FromResult(Probation.CreateUserLog(gUser));
        }

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task MessageReceived(SocketMessage message)
		{
			string chat = message.Content.ToLower();

            //Begin the GCW Barker
			if (chat.Equals("!gcwstart") && !chat.Equals("!about"))
				GCW.LoopBarker(message.Channel);

            //Stop GCW Barker
			if (chat.Equals("!gcwstop") && !chat.Equals("!about"))
			{
				GCW.IsLoopStarted = false;
				await message.Channel.SendMessageAsync("Stopping GCW Barker...");
			}

            //Sets number of minutes before a GCW battle that alert is sent
			if (chat.Contains("!settime") && !chat.Equals("!about"))
			{
				AppSettings.TimeOffset = GCW.TrimTime(chat);
                AppSettings.WriteSettings();

				await message.Channel.SendMessageAsync("Alert time set to " + AppSettings.TimeOffset + " minutes.");
			}

            //Get Help message
			if (chat.Equals("!about"))
				await message.Channel.SendMessageAsync(Help());

            //Get info on next GCW Battle
			if (chat.Equals("!gcwnext") && !chat.Equals("!about"))
			{
				var next = GCW.GCWNext();

				if (!string.IsNullOrEmpty(next))
					await message.Channel.SendMessageAsync(GCW.GCWNext());
				else
					await message.Channel.SendMessageAsync("Unable to retrieve next battle");
			}

            //Display settings
			if (chat.Equals("!status") && !chat.Equals("!about"))
				await message.Channel.SendMessageAsync(RexStatus());

            //Turn on Server monitor
            if (chat.Contains("!servermonitor") && !chat.Equals("!about"))
                ServerMonitor.LoopServerStatus(message.Channel);

            //Get current server status
            if (chat.Equals("!serverstatus") && !chat.Equals("!about"))
                await message.Channel.SendMessageAsync("The server is currently " + ServerMonitor.SWGStatus().ToLower());

            //Get user's probation info
            if (chat.Contains("!getprob") && !chat.Equals("!about"))
                if (message.Author.Id == 275073686661234688)
                    await message.Channel.SendMessageAsync(Probation.GetUserJoinedDetails(chat));
            
            //Set number of probation days
            if (chat.Contains("!problength") && !chat.Equals("!about"))
                await message.Channel.SendMessageAsync(Probation.SetProbationPeriod(chat));
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
			sb.Append(AppSettings.TimeOffset);
			sb.Append(" minutes.\n");

            if (ServerMonitor.isServerMonitorOn)
                sb.Append("Server monitor is on.");
            else
                sb.Append("Server monitor is off.");

			return sb.ToString();
		}

        private async Task ClientReady()
        {
            //Load Server Settings
            AppSettings.LoadUserLogs();
            AppSettings.LoadSettings();
            Probation.StartProbationLoop();

            var channel = AppSettings.Client.GetGuild(663179986441732131).GetChannel(807743051798675473) as ISocketMessageChannel;
            channel.SendMessageAsync("Settings loaded");
        }

        public static void SendMessageToChannel(string message)
        {
            try
            {
                var channel = AppSettings.Client.GetGuild(663179986441732131).GetChannel(807743051798675473) as ISocketMessageChannel;
                channel.SendMessageAsync(message);
            }
            catch (Exception a)
            {
                Console.WriteLine("Program.SendMessageToChannel Failed: " + a.Message);
                throw;
            }
        }
    }
}
