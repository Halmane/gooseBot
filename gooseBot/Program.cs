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
    var client = servicesProvider.GetService<DiscordSocketClient>();

    var sqliteLogger = servicesProvider.GetService<SqliteLogger>();
    servicesProvider.GetService<Logger>();

    var interactionService = new InteractionService(client.Rest);

    try
    {
        client.Ready += async () =>
        {
            await interactionService.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                servicesProvider
            );
            await interactionService.RegisterCommandsGloballyAsync();
        };
    }
    catch
    {
        client.Log += (log) =>
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        };
    }

    client.Log += (log) =>
    {
        sqliteLogger.LogIntoDbAsync(log.Source, log.Message);
        return Task.CompletedTask;
    };

    client.SlashCommandExecuted += async (interaction) =>
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(client, interaction);
        await interactionService.ExecuteCommandAsync(ctx, servicesProvider);
    };

    using (FileStream fs = new FileStream("DiscordKey.txt", FileMode.OpenOrCreate))
    {
        await client.LoginAsync(TokenType.Bot, JsonSerializer.Deserialize<string>(fs));
    }

    await client.StartAsync();
    await Task.Delay(-1);
}
