using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System.ComponentModel.Design;

namespace gooseBot;

public class CommandGroupModule
    : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    [SlashCommand("echo", "echo")]
    public async Task EchoAsync(string echo)
    {
        await RespondAsync(echo);
        
    }

    [SlashCommand("list-roles", "list-roles")]
    public async Task ShowListOfRolesAsync(IUser user)
    {
        var guildUser = (SocketGuildUser)user;
        var roleList = string.Join(
            ",\n",
            guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention)
        );

        var embedBuiler = new EmbedBuilder()
            .WithAuthor(
                guildUser.ToString(),
                guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl()
            )
            .WithTitle("Roles")
            .WithDescription(roleList)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build());
    }

    [SlashCommand("change-roles", "change-roles")]
    public async Task ChangeRoleAsync(IUser user, IRole role)
    {
        var guildUser = (SocketGuildUser)user;
        foreach (var Role in guildUser.Roles)
        {
            if (Role == role)
            {
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Role removed");
                return;
            }
        }

        await guildUser.AddRoleAsync(role);
        await RespondAsync("Role added");
    }

    [SlashCommand("delete-message", "delete-message")]
    public async Task DeleteMessages(int count)
    {
        var messageList = Context.Channel.GetCachedMessages(count);
        foreach (var message in messageList) 
        {
            await Context.Channel.DeleteMessageAsync(message.Id);
        }
    }
}
