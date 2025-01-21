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
    public async Task<IEnumerable<UserWithRolesGetDto>> GetAllUsersAsync(CancellationToken cancellationToken) {
        return await _userWithRolesService.GetAllUsersAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<UserWithRolesGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        return await _userWithRolesService.GetUserAsync(id, cancellationToken);
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