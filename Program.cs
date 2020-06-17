using System;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Services;
using System.Xml.Xsl;

namespace DiscordBot
{
    class Program
    {
        //setup our fields we assign later
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();    
        }

        public Program()
        {
            //creat ethe configuration
            var _builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(path: "config.json");
            

            //build the configuration and assign to config
            _config = _builder.Build();
            
        }

        public async Task MainAsync()
        {
            //call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using(var services = ConfigureServices())
            {
                //get the client and assign to client
                //you get thre services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                var audioService = services.GetRequiredService<AudioService>();

                //setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                

                //this is where we get the token value for the configuration file and start the bot
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                //we get the command handler class here and call the initializeAsync method to start thinds up for the commandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        //this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            //this returns a ServiceProvider that is used later to call for those services
            //we can add types we hace access to here, hence adding the new using statement:
            //using DiscordBot.Services;
            //the config we build is also added wich comes in handy for setting the command prefix
            return new ServiceCollection().AddSingleton(_config).AddSingleton<DiscordSocketClient>().AddSingleton<CommandService>().AddSingleton<CommandHandler>().AddSingleton<AudioService>().BuildServiceProvider();
        }

        
    }
}
