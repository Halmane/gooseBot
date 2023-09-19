using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using gooseBot;

var sqliteController = new SqliteLogger("D:\\Huita\\gooseBot\\LogBD.db");
var _client = new DiscordSocketClient(
        new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All }
    );
var services = new ServiceCollection();
var servicesProvider = services.AddSingleton<Logger>()
                               .AddSingleton(sqliteController)
                               .AddSingleton(_client)
                               .BuildServiceProvider();
await MainAsync();

async Task MainAsync()
{
    var _interactionService = new InteractionService(_client.Rest);
    try
    {
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), servicesProvider);
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

    servicesProvider.GetService<Logger>();

    _client.Log += (log) =>
    {
        sqliteController.LogIntoDb(log.Source,log.Message);
        return Task.CompletedTask;
    };
    
    _client.SlashCommandExecuted += async (interaction) =>
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, interaction);
        await _interactionService.ExecuteCommandAsync(ctx, servicesProvider);
    };

    await _client.LoginAsync(
        TokenType.Bot,
        "MTE1MTg3MTkzNzMxNjkzMzY1Mg.Gd3LUu.r1tuWRe6e_DvElG4l-6ilZRoyKm6W3sUIkR9Ho"
    );
    await _client.StartAsync();
    await Task.Delay(-1);
}


