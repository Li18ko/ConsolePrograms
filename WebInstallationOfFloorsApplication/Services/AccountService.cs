using System.Security.Cryptography;
using System.Text;
using Log;

namespace WebInstallationOfFloorsApplication;

public class AccountService {
    private readonly Logger _logger;
    private readonly UserRepository _userRepository;
    private readonly TokenService _tokenService;
    
    public AccountService(Logger logger, UserRepository userRepository, TokenService tokenService) {
        _logger = logger;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }
    
    public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password)) {
            _logger.Warning("Некорректные данные для входа");
            return null;
        }

        _logger.Debug("Попытка входа пользователя с логином: " + request.Login);

        var user = await _userRepository.GetUserByLoginAsync(request.Login, cancellationToken);

        if (user == null || !VerifyPassword(request.Password, user.Password)) {
            _logger.Warning("Неверные данные для входа для пользователя: " + request.Login);
            return null;
        }

        _logger.Debug("Пользователь успешно авторизован: " + request.Login);
        
        var userRoles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
        
        var accessToken = await _tokenService.GenerateAccessToken(user, userRoles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        await _userRepository.SaveRefreshTokenAsync(user, refreshToken, cancellationToken);

        return new TokenResponseDto {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken) {
        _logger.Debug("Процесс выхода из системы");

        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
        
        if (user != null) {
            await _userRepository.RevokeRefreshTokensAsync(user, cancellationToken);
            
            _logger.Debug("RefreshToken аннулирован для пользователя: " + user.Login);
            return true;
        }

        _logger.Warning("Не удалось найти пользователя с указанным refreshToken.");
        return false;
    }
    
    public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken) {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
    
        if (user == null || user.RefreshTokens.All(t => t.IsRevoked)) {
            _logger.Warning("Refresh Token не найден или отозван.");
            return null; 
        }

        _logger.Debug("Refresh Token найден, генерируем новый Access Token");
        
        var userRoles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);

        var newAccessToken = await _tokenService.GenerateAccessToken(user, userRoles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        await _userRepository.RevokeRefreshTokensAsync(user, cancellationToken);
        await _userRepository.SaveRefreshTokenAsync(user, newRefreshToken, cancellationToken);
    
        return new TokenResponseDto {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
    
    public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken) {
        return await _userRepository.GetUserPermissionsAsync(userId, cancellationToken);
    }
    
    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId, CancellationToken cancellationToken) {
        return await _userRepository.GetUserRolesAsync(userId, cancellationToken);
    }

    private bool VerifyPassword(string inputPassword, string storedPasswordHash) {
        string inputPasswordHash =
            Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(inputPassword)));
        return inputPasswordHash == storedPasswordHash; 
    }
}