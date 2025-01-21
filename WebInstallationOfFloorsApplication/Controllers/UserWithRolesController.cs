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
        var user = new User {
            Name = dto.Name,
            Login = dto.Login,
            Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(dto.Password))),
            ChatId = dto.ChatId,
            UserRoles = new List<UserRole>()
        };
        
        if (dto.RoleIds != null) {
            foreach (var roleId in dto.RoleIds) {
                user.UserRoles.Add(new UserRole {
                    RoleId = roleId,
                    User = user
                });
            }
        }
        return await _userWithRolesService.InsertUserAsync(user, cancellationToken);
    }

    [HttpPut]
    public async Task<UserWithRolesUpdateDto> UpdateUserAsync(UserWithRolesUpdateDto dto, CancellationToken cancellationToken) {
        var updatedUser = await _userWithRolesService.UpdateUserAsync(dto, cancellationToken);
    
        if (updatedUser == null) {
            return null;
        }

        return new UserWithRolesUpdateDto {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Login = updatedUser.Login,
            ChatId = updatedUser.ChatId,
            RoleIds = updatedUser.UserRoles.Select(ur => ur.RoleId).ToList(),
            Password = " "
        };
    }


    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        await _userWithRolesService.DeleteUserAsync(id, cancellationToken);
    }

}