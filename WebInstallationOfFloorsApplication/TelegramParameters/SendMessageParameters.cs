using System.Text.Json.Serialization;

namespace WebInstallationOfFloorsApplication;

public class SendMessageParameters: TelegramParameters {
    [JsonPropertyName("reply_markup")]
    public ReplyMarkup ReplyMarkupData { get; set; }
    
    public SendMessageParameters(long chatId, string text, string parseMode, ReplyMarkup replyMarkup) : base(chatId, text, parseMode) {
        ChatId = chatId;
        Text = text;
        ParseMode = parseMode;
        ReplyMarkupData = replyMarkup;
    }
}

public class ReplyMarkup {
    [JsonPropertyName("inline_keyboard")]
    public List<List<InlineKeyboardButton>> InlineKeyboard { get; set; }
    
    public ReplyMarkup(List<List<InlineKeyboardButton>> inlineKeyboard) {
        InlineKeyboard = inlineKeyboard;
    }

}

public class InlineKeyboardButton {
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("callback_data")]
    public string CallbackData { get; set; }

    public InlineKeyboardButton(string text, string callbackData) {
        Text = text;
        CallbackData = callbackData;
    }
}