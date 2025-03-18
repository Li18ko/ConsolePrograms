using System;
using System.ComponentModel.DataAnnotations;

namespace WebInstallationOfFloorsApplication;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class TelegramChatIdAttribute: ValidationAttribute{
    
    public TelegramChatIdAttribute() {
        ErrorMessage = "ID чата Телеграма должен быть отрицательным числом (обычная группа) или начинаться с -100 (супергруппа/канал).";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value == null) return new ValidationResult("ID чата обязателен.");;

        if (value is long chatId) {
            string chatIdStr = chatId.ToString();

            if (chatId < 0 && chatId > -999999999999) {
                return ValidationResult.Success;
            }

            if (chatIdStr.StartsWith("-100") && chatId >= -1997852516352 && chatId <= -1000000000001) {
                return ValidationResult.Success;
            }

            return new ValidationResult("Некорректный ID чата. Группа должна быть отрицательной, а супергруппа начинаться с -100");
        }

        return new ValidationResult("ID чата должен быть числом.");
    }
}
