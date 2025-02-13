using System.Text.Json.Serialization;

namespace WebInstallationOfFloorsApplication;

public class TelegramParameters {
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("parse_mode")]
    public string ParseMode { get; set; }
    
    
    public TelegramParameters(long chatId, string text, string parseMode) {
        ChatId = chatId;
        Text = text;
        ParseMode = parseMode;
    }
}