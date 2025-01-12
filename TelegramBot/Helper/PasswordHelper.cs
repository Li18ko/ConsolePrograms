using System.Security.Cryptography;
using System.Text;

namespace TelegramBot.Helper;

public class PasswordHelper {
    public static string GenerateSalt() {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes); 
    }

    public static string HashPassword(string password, string salt) {
        var saltedPassword = password + salt;

        using (var sha256 = SHA256.Create()) {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword, string salt){
        var providedHash = HashPassword(providedPassword, salt);
        return hashedPassword == providedHash;
    }
}