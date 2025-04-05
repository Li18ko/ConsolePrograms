using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {

    private readonly UserService _userService;

    public UserController(UserService userService) {
        _userService = userService;
    }

    [HasPermission("UserList")]
    [HttpGet("List")]
    public async Task<PagedResult<UserGetDto>> GetAllUsersAsync(CancellationToken cancellationToken,
        [FromQuery] UserFilterDto filter) {
        return await _userService.GetAllUsersAsync(filter, cancellationToken);
    }

    [HasPermission("UserList")]
    [HttpGet("ListWithoutSorting")]
    public async Task<IEnumerable<UserGetDto>> GetAllUsersWithoutSortingAsync(CancellationToken cancellationToken) {
        return await _userService.GetAllUsersWithoutSortingAsync(cancellationToken);
    }

    [HasPermission("User", "UserEdit")]
    [HttpGet("{id}")]
    public async Task<UserGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _userService.GetUserAsync(id, cancellationToken);
    }
    
    [HasPermission("UserAdd", "UserEdit")]
    [HttpGet("isLoginTaken/{login}/{id?}")]
    public async Task<bool> IsLoginTakenAsync(string login, CancellationToken cancellationToken, [FromRoute] int id = 0) {
        return await _userService.IsLoginTakenAsync(id == 0 ? null : id, login, cancellationToken);
    }
    
    [HasPermission("UserAdd", "UserEdit")]
    [HttpGet("isEmailTaken/{email}/{id?}")]
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken, [FromRoute] int id = 0) {
        return await _userService.IsEmailTakenAsync(id == 0 ? null : id, email, cancellationToken);
    }

    [HasPermission("UserAdd")]
    [HttpPost]
    public async Task<int?> InsertUserAsync([FromBody] UserInsertDto dto, CancellationToken cancellationToken) {
        return await _userService.InsertUserAsync(dto, cancellationToken);
    }

    [HasPermission("UserEdit")]
    [HttpPut]
    public async Task<UserUpdateDto> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken) {
        return await _userService.UpdateUserAsync(dto, cancellationToken);
    }

    [HasPermission("UserDelete")]
    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        await _userService.DeleteUserAsync(id, cancellationToken);
    }

}