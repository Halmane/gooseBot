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
        MessageUpdated();
        DeleteMessage();
    }  

    private void RoleCreated()
    {
        _client.RoleCreated += async (role) => { await _sqliteController.LogIntoDb($"Role ID:{role.Id}", $"Create Role:{role.Name}"); };
    }
    private void MessageReceived()
    {
        _client.MessageReceived += async (messsage) => { if (messsage.Content.Length > 0) await _sqliteController.LogIntoDb($"Channel:{messsage.Channel.Id} Author ID:{messsage.Author.Id}", $"Send message: {messsage.Content}"); };
    }
    private void MessageUpdated()
    {
        _client.MessageUpdated += async (oldMessage,message,channel ) => { await _sqliteController.LogIntoDb($"Channel:{channel.Id}", $"{oldMessage.Value} -> {message}"); };
    }

    private void DeleteMessage()
    {
        _client.MessageDeleted += async (message,channel) => { await _sqliteController.LogIntoDb($"Channel:{channel.Id}",$"Message delete: {message.Id}/{message.Value}"); };
    }
}
