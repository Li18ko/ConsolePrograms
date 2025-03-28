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

    [HttpGet("List")]
    public async Task<PagedResult<UserGetDto>> GetAllUsersAsync(CancellationToken cancellationToken,
        [FromQuery] UserFilterDto filter) {
        
        return await _userService.GetAllUsersAsync(filter, cancellationToken);
    }

    [HttpGet("ListWithoutSorting")]
    public async Task<IEnumerable<UserGetDto>> GetAllUsersWithoutSortingAsync(CancellationToken cancellationToken) {
        return await _userService.GetAllUsersWithoutSortingAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<UserGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _userService.GetUserAsync(id, cancellationToken);
    }
    
    [HttpGet("isLoginTaken/{login}/{id?}")]
    public async Task<bool> IsLoginTakenAsync(string login, CancellationToken cancellationToken, [FromRoute] int id = 0) {
        return await _userService.IsLoginTakenAsync(id == 0 ? null : id, login, cancellationToken);
    }
    
    [HttpGet("isEmailTaken/{email}/{id?}")]
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken, [FromRoute] int id = 0) {
        return await _userService.IsEmailTakenAsync(id == 0 ? null : id, email, cancellationToken);
    }

    [HttpPost]
    public async Task<int?> InsertUserAsync([FromBody] UserInsertDto dto, CancellationToken cancellationToken) {
        return await _userService.InsertUserAsync(dto, cancellationToken);
    }

    [HttpPut]
    public async Task<UserUpdateDto> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken) {
        return await _userService.UpdateUserAsync(dto, cancellationToken);
    }


    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        await _userService.DeleteUserAsync(id, cancellationToken);
    }

}