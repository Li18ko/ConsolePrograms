using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class UserWithRolesController : ControllerBase {

    private readonly UserWithRolesService _userWithRolesService;

    public UserWithRolesController(UserWithRolesService userWithRolesService) {
        _userWithRolesService = userWithRolesService;
    }

    [HttpGet("List")]
    public async Task<IEnumerable<UserWithRolesGetDto>> GetAllUsersAsync(CancellationToken cancellationToken,
        [FromQuery] string sort, [FromQuery] string filter = "", [FromQuery] string search = "") {
        return await _userWithRolesService.GetAllUsersAsync(sort, filter, search, cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<UserWithRolesGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _userWithRolesService.GetUserAsync(id, cancellationToken);
    }
    
    [HttpGet("isLoginTakenByOtherUser/{id}/{login}")]
    public async Task<bool> IsLoginTakenByOtherUserAsync(int id, string login, CancellationToken cancellationToken) {
        return await _userWithRolesService.IsLoginTakenByOtherUserAsync(id, login, cancellationToken);
    }
    
    [HttpGet("IsLoginTakenAsync/{login}")]
    public async Task<bool> IsLoginTakenAsync(string login, CancellationToken cancellationToken) {
        return await _userWithRolesService.IsLoginTakenAsync(login, cancellationToken);
    }
    
    [HttpGet("isEmailTakenByOtherUser/{id}/{email}")]
    public async Task<bool> IsEmailTakenByOtherUserAsync(int id, string email, CancellationToken cancellationToken) {
        return await _userWithRolesService.IsEmailTakenByOtherUserAsync(id, email, cancellationToken);
    }
    
    [HttpGet("IsEmailTakenAsync/{email}")]
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken) {
        return await _userWithRolesService.IsEmailTakenAsync(email, cancellationToken);
    }

    [HttpPost]
    public async Task<int?> InsertUserAsync([FromBody] UserWithRolesInsertDto dto, CancellationToken cancellationToken) {
        return await _userWithRolesService.InsertUserAsync(dto, cancellationToken);
    }

    [HttpPut]
    public async Task<UserWithRolesUpdateDto> UpdateUserAsync(UserWithRolesUpdateDto dto, CancellationToken cancellationToken) {
        return await _userWithRolesService.UpdateUserAsync(dto, cancellationToken);
    }


    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        await _userWithRolesService.DeleteUserAsync(id, cancellationToken);
    }

}