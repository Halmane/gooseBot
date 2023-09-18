using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using gooseBot;

var services = new ServiceCollection();
var servicesProvider = services.BuildServiceProvider();
DiscordSocketClient _client;
await MainAsync();

async Task MainAsync()
{
    _client = new DiscordSocketClient(
        new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All }
    );

    var sqliteController = new SqliteController("D:\\Huita\\gooseBot\\LogBD.db");
    var _interactionService = new InteractionService(_client.Rest);
    try
    {
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services: null);
            await _interactionService.RegisterCommandsGloballyAsync();
        };
    }
    catch
    {
        _client.Log += (log) =>
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        };
    }
    
    _client.Log += (log) =>
    {
        sqliteController.Logger(log.Source,log.Message);
        return Task.CompletedTask;
    };
    _client.MessageReceived += async (messsage) => { if(messsage.Content.Length>0) sqliteController.Logger($"Channel:{messsage.Channel.Id} Author ID:{messsage.Author.Id}",$"Send message: {messsage.Content}"); };
    _client.RoleCreated += async (messsage) => { sqliteController.Logger($"Role ID:{messsage.Id}", $"Create Role:{messsage.Name}");};
    _client.SlashCommandExecuted += async (interaction) =>
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, interaction);
        await _interactionService.ExecuteCommandAsync(ctx, servicesProvider);
    };

    await _client.LoginAsync(
        TokenType.Bot,
        "MTE1MTg3MTkzNzMxNjkzMzY1Mg.GXluRj.7ofg9v4LGLkcmUnPXnm9y4lZR2dhptoz4z3znI"
    );
    await _client.StartAsync();
    await Task.Delay(-1);
}


