using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace GhostpingLogger {
    class Client {
        public DiscordClient dclient;
        public static DiscordChannel logsChannel;
        public static ConfigJson cfgjson;

        public struct ConfigJson {
            [JsonProperty("token")]
            public string token;

            [JsonProperty("prefix")]
            public string prefix;
        }

        public async Task SetupClient() {
            FileStream fileStream = File.OpenRead("config.json");
            string json = await new StreamReader(fileStream).ReadToEndAsync();

            cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
            DiscordConfiguration cfg = new DiscordConfiguration {
                Token = cfgjson.token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
            
            dclient = new DiscordClient(cfg);
            
            dclient.Ready += Events.Dclient_Ready;
            dclient.GuildAvailable += Events.Dclient_GuildAvailable;
            dclient.ClientErrored += Events.Dclient_ClientError;
            dclient.MessageCreated += Events.Dclient_MessageCreated;
            dclient.MessageDeleted += Events.Dclient_MessageDeleted;
            dclient.MessageUpdated += Events.Dclient_MessageUpdated;
        }

        public async Task ConnectClient() {
            DiscordActivity activity = new DiscordActivity();
            activity.ActivityType = ActivityType.Streaming;
            activity.Name = "ghostpings to staff.";
            activity.StreamUrl = "https://www.twitch.tv/anomaly";

            await dclient.ConnectAsync(activity);
            await Task.Delay(-1);
        }
    }
}
