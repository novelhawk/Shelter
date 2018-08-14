using Mod.Discord.Converters;
using Mod.Discord.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mod.Discord.RPC.Payload
{
    /// <summary>
    /// Used for Discord IPC Events
    /// </summary>
    internal class EventPayload : IPayload
    {
        /// <summary>
        /// The data the server sent too us
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Data Data { get; set; }

        /// <summary>
        /// The type of event the server sent
        /// </summary>
        [JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseConverter))]
        public ServerEvent? Event { get; set; }

        public EventPayload() : base() { Data = null; }
        public EventPayload(long nonce) : base(nonce) { Data = null; }

        public override string ToString()
        {
            return "Event " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
        }
    }

    public class Data
    {
        public ErrorMessage ErrorMessage => new ErrorMessage
        {
            Code = Code,
            Message = Message
        };

        public ReadyMessage ReadyMessage => new ReadyMessage
        {
            Configuration = Configuration,
            User = User,
            Version = Version
        };

        public RichPresenceResponse RichPresenceResponse => new RichPresenceResponse
        {
            ClientID = ClientID,
            Name = Name
        };
        
        
        // Error message
        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public Message.ErrorCode Code { get; internal set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; internal set; }
        
        
        // Redy message
        [JsonProperty("config", NullValueHandling = NullValueHandling.Ignore)]
        public Configuration Configuration { get; internal set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public User User { get; internal set; }

        [JsonProperty("v", NullValueHandling = NullValueHandling.Ignore)]
        public int Version { get; internal set; }
        
        
        // RichPresenceResponse
        [JsonProperty("application_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ClientID;

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name;
        
        //JoinMessage
        [JsonProperty("secret")]
        public string Secret { get; internal set; }
        
        
        // evt
        [JsonProperty("evt", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(EnumSnakeCaseConverter))]
        public ServerEvent? Event { get; internal set; }
    }
}
