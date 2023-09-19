using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using gooseBot;

namespace gooseBot;

public class Logger
{
    private SqliteLogger _sqliteController;
    private DiscordSocketClient _client;
    public Logger(DiscordSocketClient client, SqliteLogger sqliteController)
    {
        _client = client;
        _sqliteController = sqliteController;
        RoleCreated();
        MessageReceived();
    }  

    private void RoleCreated()
    {
        _client.RoleCreated += async (messsage) => { _sqliteController.LogIntoDb($"Role ID:{messsage.Id}", $"Create Role:{messsage.Name}"); };
    }
    private void MessageReceived()
    {
        _client.MessageReceived += async (messsage) => { if (messsage.Content.Length > 0) _sqliteController.LogIntoDb($"Channel:{messsage.Channel.Id} Author ID:{messsage.Author.Id}", $"Send message: {messsage.Content}"); };
    }
}
