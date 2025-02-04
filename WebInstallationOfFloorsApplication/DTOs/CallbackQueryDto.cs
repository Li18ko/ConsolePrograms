using System.Text.Json.Serialization;

namespace WebInstallationOfFloorsApplication {
    public class CallbackQueryDto {
        
        [JsonPropertyName("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }

    public class CallbackQuery {
        [JsonPropertyName("message")]
        public MessageDetails Message { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

    public class MessageDetails {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("chat")]
        public ChatDetails Chat { get; set; }
    }
    
    public class ChatDetails {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
    
    public class DataDetails {
        [JsonPropertyName("action")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Action Action { get; set; }

        [JsonPropertyName("taskId")]
        public long TaskId { get; set; }
    }
    
    public enum Action {
        Open,
        Done,
        Reject
    }
}