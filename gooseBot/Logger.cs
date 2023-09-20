using Discord;
using Discord.WebSocket;


namespace gooseBot;

public class Logger 
{
    private SqliteLogger _sqliteController;
    private DiscordSocketClient _client;
    public Logger(DiscordSocketClient client, SqliteLogger sqliteController)
    {
        _client = client;
        _sqliteController = sqliteController;
        AddEventHandlerForRoleCreated();
        AddEventHandlerForMessageReceived();
        AddEventHandlerForMessageUpdated();
        AddEventHandlerForDeleteMessage();
    }  

    private void AddEventHandlerForRoleCreated()
    {
        _client.RoleCreated += async (role) => { await _sqliteController.LogIntoDbAsync($"Role ID:{role.Id}", $"Create Role:{role.Name}"); };
    }
    private void AddEventHandlerForMessageReceived()
    {
        _client.MessageReceived += async (message) => { if (message.Content.Length > 0) await _sqliteController.LogIntoDbAsync($"Channel:{message.Channel.Id} Author ID:{message.Author.Id}", $"Send message: {message.Content}"); };
    }
    private void AddEventHandlerForMessageUpdated()
    {
        _client.MessageUpdated += async (oldMessage,message,channel ) => { await _sqliteController.LogIntoDbAsync($"Channel:{channel.Id} Author ID:{message.Author.Id}", $"{oldMessage.Id} -> {message}"); };
    }

    private void AddEventHandlerForDeleteMessage()
    {
        _client.MessageDeleted += async (message,channel) => { await _sqliteController.LogIntoDbAsync($"Channel:{channel.Id}",$"Message delete: {message.Id}"); };
    }
}
