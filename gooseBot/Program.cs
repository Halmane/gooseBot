using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using gooseBot;
using System.Text.Json;

var services = new ServiceCollection();
using (
    var servicesProvider = services
        .AddSingleton<Logger>()
        .AddSingleton(new SqliteLogger("D:\\C#Project\\Bot\\gooseBot\\LogBD.db"))
        .AddSingleton(
            new DiscordSocketClient(
                new DiscordSocketConfig() { GatewayIntents = GatewayIntents.All }
            )
        )
        .BuildServiceProvider()
)
{
    var _client = servicesProvider.GetService<DiscordSocketClient>();

    var sqliteController = servicesProvider.GetService<SqliteLogger>();

    var _interactionService = new InteractionService(_client.Rest);

    try
    {
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                servicesProvider
            );
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
        sqliteController.LogIntoDbAsync(log.Source, log.Message);
        return Task.CompletedTask;
    };

    _client.SlashCommandExecuted += async (interaction) =>
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, interaction);
        await _interactionService.ExecuteCommandAsync(ctx, servicesProvider);
    };

    using (FileStream fs = new FileStream("DiscordKey.txt", FileMode.OpenOrCreate))
    {
        await _client.LoginAsync(TokenType.Bot, JsonSerializer.Deserialize<string>(fs));
    }

    await _client.StartAsync();
    await Task.Delay(-1);
}
