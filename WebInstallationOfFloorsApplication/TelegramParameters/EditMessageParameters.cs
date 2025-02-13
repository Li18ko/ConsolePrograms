using System.Text.Json.Serialization;

namespace WebInstallationOfFloorsApplication;

public class EditMessageParameters: TelegramParameters {
    [JsonPropertyName("message_id")]
    public long MessageId { get; set; }
    
    public EditMessageParameters(long chatId, string text, string parseMode, long messageId) : base(chatId, text, parseMode) {
        ChatId = chatId;
        Text = text;
        ParseMode = parseMode;
        MessageId = messageId;
    }
}