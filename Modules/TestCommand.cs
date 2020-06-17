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
using System.ComponentModel.Design;


namespace DiscordBot.Modules
{
    public class TestCommand : ModuleBase
    {  
        [Command("test")]
        public async Task TestCommandInOtherFiles(int num, [Remainder]string args = null)
        {
            var sb = new StringBuilder();

            if (args == null)
                sb.AppendLine("You wrote nothing");
            else
                sb.AppendLine($"You wrote {args} and number {num}");

            await ReplyAsync(sb.ToString());
        }

        
    }
}
