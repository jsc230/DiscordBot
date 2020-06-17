using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace DiscordBot.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if(ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if(target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();
            _audioClients.TryAdd(guild.Id, audioClient);

            if(ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                System.Console.WriteLine($"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;

            if(ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                System.Console.WriteLine($"Disconnected from voice channel on {guild.Name}.");
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            //get a full path to the file if the value for path is only a filename
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist!");
                return;
            }
            IAudioClient client;
            if(ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                System.Console.WriteLine($"Starting playback of {path} in {guild.Name}.");
                using (Stream output = CreateProcess(path).StandardOutput.BaseStream)
                using (AudioOutStream stream = client.CreateDirectPCMStream(AudioApplication.Music))
                {
                    try { await stream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-loglevel panic -i \"{path}\" -ac 2 -f s161e -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
