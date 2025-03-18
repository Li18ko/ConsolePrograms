using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class RoleController: ControllerBase {

    private readonly RoleService _roleService;

    public RoleController(RoleService roleService) {
        _roleService = roleService;
    }
    
    [HttpGet("List")]
    public async Task<IEnumerable<RoleGetDto>> GetAllRolesAsync(CancellationToken cancellationToken, [FromQuery] IEnumerable<bool>? status = null) {
        return await _roleService.GetAllRolesAsync(status, cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<RoleGetDto> GetRoleAsync(int id, CancellationToken cancellationToken) {
        return await _roleService.GetRoleAsync(id, cancellationToken);
    }
    
    [HttpPost]
    public async Task<int?> InsertRoleAsync([FromBody] RoleInsertDto dto, CancellationToken cancellationToken) {
        return await _roleService.InsertRoleAsync(dto, cancellationToken);
    }

    [HttpPut]
    public async Task<RoleUpdateDto> UpdateRoleAsync(RoleUpdateDto dto, CancellationToken cancellationToken) {
        return await _roleService.UpdateRoleAsync(dto, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteRoleAsync(int id, CancellationToken cancellationToken) {
        await _roleService.DeleteRoleAsync(id, cancellationToken);
    }
    
    [HttpGet("ListFunctions")]
    public async Task<IEnumerable<Function>> GetAllFunctionsAsync(CancellationToken cancellationToken) {
        return await _roleService.GetAllFunctionsAsync(cancellationToken);
    }

}