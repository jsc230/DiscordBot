using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules
{
    public class BasicCommands : ModuleBase
    {
        [Command("help")]
        public async Task HelpCommand()
        {
            var sb = new StringBuilder();

            sb.AppendLine("The following commands are implemented:");
            sb.AppendLine("-hello -> Says hello");
            sb.AppendLine("-time -> Gives time in EST");
            sb.AppendLine("-gretchen -> Gretchen!");
            sb.AppendLine("-guessgame -> Number guessing game");

            await ReplyAsync(sb.ToString());
        }

        [Command("hello")]
        public async Task HelloCommand()
        {
            //initialize empty string builder for reply
            var sb = new StringBuilder();

            //get user info from context
            var user = Context.User;
            
            //build out the reply                       
            sb.AppendLine($"[{user.Username}] says hello.");

            //send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("time")]
        public async Task TimeCommand()
        {
            var sb = new StringBuilder();
            TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime dateTime_Eastern = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);

            sb.AppendLine("The time is: " + dateTime_Eastern + " EST");

            await ReplyAsync(sb.ToString());
            
        }

        [Command("gretchen")]
        public async Task GretchenCommand()
        {
            var embed = new EmbedBuilder();

            embed.WithImageUrl("https://i.imgur.com/eqDvPtm.jpg");

            await ReplyAsync("Gretchen!", false, embed.Build());
        }

    }
}
