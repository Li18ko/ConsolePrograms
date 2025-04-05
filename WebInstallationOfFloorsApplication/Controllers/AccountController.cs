using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase {
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService) {
        _accountService = accountService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<TokenResponseDto?> LoginAsync([FromBody] LoginRequestDto request, CancellationToken cancellationToken) {
        return await _accountService.LoginAsync(request, cancellationToken);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async System.Threading.Tasks.Task LogoutAsync([FromBody] string refreshToken, CancellationToken cancellationToken) {
       await _accountService.LogoutAsync(refreshToken, cancellationToken);
    }
    
    [Authorize]
    [HttpPost("refresh")]
    public async Task<TokenResponseDto?> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken) {
        return await _accountService.RefreshTokenAsync(refreshToken, cancellationToken);
    }
    
    [Authorize]
    [HttpPost("userPermissions")]
    public async Task<IEnumerable<string>> GetUserPermissionsAsync([FromBody] int id, CancellationToken cancellationToken) {
        return await _accountService.GetUserPermissionsAsync(id, cancellationToken);
    }
}