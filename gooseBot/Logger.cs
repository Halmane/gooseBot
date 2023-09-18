using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using gooseBot;

namespace gooseBot;

public class Logger
{
    private DiscordSocketClient _client;
    public Logger(DiscordSocketClient client)
    {
        _client = client;

    }


    
}
