using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GhostpingLogger {
    class Events {
        public static async Task Dclient_MessageDeleted(MessageDeleteEventArgs e) {
            if (e.Message.Content.Contains("<@") && e.Message.Content.Contains(">")) {
                string match = Regex.Match(e.Message.Content, @"<@(.+?)>").Groups[1].Value;
                if (match == "")
                    match = Regex.Match(e.Message.Content, @"<@!(.+?)>").Groups[1].Value;
                if ((await e.Channel.Guild.GetMemberAsync(ulong.Parse(match))) == null)
                    return;

                if (match != "" && match != "!") {
                    DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
                        Color = new DiscordColor(255, 0, 0),
                        Title = "Ghostping detected!\nUsername and ID of ghostping author in embed footer.",
                        Description = $"Message ID: {e.Message.Id}\n\n`{e.Message.Content.Replace("`", "(grave)")}` in #{e.Message.Channel.Name}",
                        Timestamp = DateTime.UtcNow
                    };
                    embedBuilder.WithFooter($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} (User ID: {e.Message.Author.Id})", e.Message.Author.AvatarUrl);

                    await Client.logsChannel.SendMessageAsync(null, false, embedBuilder.Build());
                }
            }
        }

        public static async Task Dclient_MessageUpdated(MessageUpdateEventArgs e) {
            if (e.MessageBefore.Content.Contains("<@") && e.MessageBefore.Content.Contains(">")) {
                var msgBefore = e.MessageBefore.Content.Replace("!", "");
                var msg = e.Message.Content.Replace("!", "");

                var matchBefore = Regex.Match(msgBefore, @"<@(.+?)>").Groups[1].Value;
                if (matchBefore == "")
                    return;
                if ((await e.Channel.Guild.GetMemberAsync(ulong.Parse(matchBefore))) == null)
                    return;

                var match = Regex.Match(msg, @"<@(.+?)>").Groups[1].Value;

                if (match != matchBefore) {
                    File.AppendAllText(
                        "logs.txt",
                        $"[{DateTime.Now}] Ghostping on guild \"{e.Guild.Name}\" (ID: {e.Guild.Id}):\n\nBefore: {e.MessageBefore.Content}\n\nAfter: {e.Message.Content}\n\n");

                    DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
                        Color = new DiscordColor(255, 0, 0),
                        Title = "Ghostping detected!\nUsername and ID of ghostping author in embed footer.",
                        Description = $"Message ID: {e.Message.Id}\n\nBefore: `{e.MessageBefore.Content.Replace("`", "(grave)")}`\nAfter: `{e.Message.Content.Replace("`", "(grave)")}`\nin #{e.Message.Channel.Name}",
                        Timestamp = DateTime.UtcNow
                    };
                    embedBuilder.WithFooter($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} (User ID: {e.Message.Author.Id})", e.Message.Author.AvatarUrl);

                    await Client.logsChannel.SendMessageAsync(null, false, embedBuilder.Build());
                }
            }
        }

        public static async Task Dclient_MessageCreated(MessageCreateEventArgs e) {
            if (e.Message.Content == $"{Client.cfgjson.prefix}setup") {
                foreach (DiscordRole role in (await e.Message.Channel.Guild.GetMemberAsync(e.Message.Author.Id)).Roles) {
                    if ((role.CheckPermission(Permissions.ManageGuild) == PermissionLevel.Allowed) ||
                        (role.CheckPermission(Permissions.ManageChannels) == PermissionLevel.Allowed) ||
                        (role.CheckPermission(Permissions.Administrator) == PermissionLevel.Allowed))
                    {
                        Client.logsChannel = e.Message.Channel;

                        DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
                            Color = new DiscordColor(0, 255, 0),
                            Title = "Bot has been setup to log ghostpings in current channel.",
                            Timestamp = DateTime.UtcNow
                        };
                        embedBuilder.WithFooter(e.Client.CurrentUser.Username, e.Client.CurrentUser.AvatarUrl);
                        await Client.logsChannel.SendMessageAsync(null, false, embedBuilder.Build());

                        return;
                    }
                }

                DiscordEmbedBuilder embedBuilderOwner = new DiscordEmbedBuilder {
                    Color = new DiscordColor(0, 255, 0),
                    Title = "You do not have permission to setup the bot in this channel.",
                    Timestamp = DateTime.UtcNow
                };
                embedBuilderOwner.WithFooter(e.Client.CurrentUser.Username, e.Client.CurrentUser.AvatarUrl);
                await e.Message.Channel.SendMessageAsync(null, false, embedBuilderOwner.Build());
            }
        }

        public static Task Dclient_ClientError(ClientErrorEventArgs e) {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Error,
                "GhostpingLogger",
                $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}\n\n{e.Exception.InnerException}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        public static Task Dclient_GuildAvailable(GuildCreateEventArgs e) {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Info,
                "GhostpingLogger",
                $"Guild available: {e.Guild.Name} (ID: {e.Guild.Id})",
                DateTime.Now);
            return Task.CompletedTask;
        }

        public static Task Dclient_Ready(ReadyEventArgs e) {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Info,
                "GhostpingLogger",
                "Client is ready to process events.",
                DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
