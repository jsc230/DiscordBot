using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Services;
using Discord;
using System.Threading.Channels;

namespace DiscordBot.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

        //must be marked with 'RunMode.Async
        //otherwise the bot will not repsond until the task times out
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCommand()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCommand()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCommand([Remainder] string song)
        {
            
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }
    }
}
