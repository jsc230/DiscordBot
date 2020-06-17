using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Collections.Generic;


namespace DiscordBot.Services
{
    class CommandHandler : ModuleBase
    {
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
              

        //List<PhraseWords> phraseWords = new List<PhraseWords>();
        List<string> phraseWords = new List<string>();
        List<string> phraseReply = new List<string>();

        public CommandHandler (IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

                       
            
            //take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;

            //take  action when we recievea message (so we can process it and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
            _client.UserJoined += AnnounceJoinedUser;   //check if user joined
            
        }

        public async Task InitializeAsync()
        {
            //register modules that are public and inherit ModuleBase<T>
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            //create lists for the keywords and the phrases from phrases.txt
            string line;
            System.IO.StreamReader phrases = new System.IO.StreamReader("phrases.txt");

            while((line = phrases.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                phraseWords.Add(parts[0]);
                phraseReply.Add(parts[1]);
            }
        }

        //take actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            //ensures we done process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }
            if (message.Source != MessageSource.User)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            //go to swearing method
            if(_config["Swearing"] == "false")
            {
                await SwearWord(context, rawMessage);
            }

            //go to phrases method
            await PhraseCheck(context, rawMessage);
                        
            //sets the argument position away from the prefix we set
            var argPos = 0;

            //get prefix for the configuration file
            char prefix = Char.Parse(_config["Prefix"]);

            //determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

                        
            //execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            //if command isn't found
            if (!command.IsSpecified)
            {
                System.Console.WriteLine($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }

            //log success to the console and exit this method
            if (result.IsSuccess)
            {
                System.Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.User.Username}]");
                return;
            }

            //failure scenario, lets the user know
            await context.Channel.SendMessageAsync($"Sorry, {context.User.Username}... something went wrong -> [{result}]!");
        }

        private async Task SwearWord(ICommandContext context, SocketMessage message)
        {
            string[] swearwords = File.ReadAllLines("swears.txt");

            if (swearwords.Any(word => message.Content.IndexOf(word, 0, message.Content.Length, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                await message.DeleteAsync();
                await context.Channel.SendMessageAsync($"{context.User.Username}, STOP SWEARING!");
            }            
        }

        private async Task PhraseCheck(ICommandContext context, SocketMessage message)
        {
            string messageData = message.Content;
            string[] parts = messageData.Split(' ');

            for(int i = 0; i < parts.Length; i++)
            {
                for(int j = 0; j < phraseWords.Count; j++)
                {
                    if (parts[i].ToLower().Equals(phraseWords[j]))
                    {
                        await context.Channel.SendMessageAsync(phraseReply[j]);
                    }
                }
            }           
            
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user) //welcomes new players
        {
            var channel = _client.GetChannel(716994181935136850) as SocketTextChannel;
            await channel.SendMessageAsync("Welcome " + user.Mention + " to the server!");
        }
    }

    
}
