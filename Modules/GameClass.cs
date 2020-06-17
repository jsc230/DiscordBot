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
    public static class GlobalVariables
    {
        public static bool guessRunning { get; set; } = false;
        public static int number;
    }
    public class GameClass : ModuleBase
    {        
        public Random random;
        
        /// <summary>
        /// Guessing Game
        /// </summary>
        /// <returns></returns>
        [Command("guessgame")]
        public async Task GuessGameCommand()
        {
            StringBuilder sb = new StringBuilder();

            random = new Random();
            GlobalVariables.number = random.Next(1, 100);
            sb.AppendLine("Guess a number between 1 and 100 using the command \"-guess number\"");
            GlobalVariables.guessRunning = true;
            
            await ReplyAsync(sb.ToString());
        }

        [Command("guess")]
        public async Task GuessCommand(int num)
        {
            string answer = "";
            
            if (GlobalVariables.guessRunning)
            {
                if(num > GlobalVariables.number)
                {
                    answer = "Lower";
                }
                if (num < GlobalVariables.number)
                {
                    answer = "Higher";
                }
                if(num == GlobalVariables.number)
                {
                    answer = "Winner";
                    GlobalVariables.guessRunning = false;
                }
            }
            else
            {
                answer = "Game not running";
            }

            await ReplyAsync(answer);
        }
        
        ////
        ///
        ////

    }
}
