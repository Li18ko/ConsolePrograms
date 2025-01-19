using System.Security.Cryptography;
using System.Text;

namespace TelegramBot.Helper;

public class PasswordHelper {
    public static string HashPassword(string password) {
        using (var sha256 = SHA256.Create()) {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string hashedPassword1, string hashedPassword2) {
        return hashedPassword1 == hashedPassword2;
    }
}