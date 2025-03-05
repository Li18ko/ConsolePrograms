using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class RoleWithFunctionsController: ControllerBase {

    private readonly RoleWithFunctionsService _roleWithFunctionsService;

    public RoleWithFunctionsController(RoleWithFunctionsService roleWithFunctionsService) {
        _roleWithFunctionsService = roleWithFunctionsService;
    }
    
    [HttpGet("List")]
    public async Task<IEnumerable<RoleWithFunctionsGetDto>> GetAllRolesAsync(CancellationToken cancellationToken) {
        return await _roleWithFunctionsService.GetAllRolesAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<RoleWithFunctionsGetDto> GetRoleAsync(int id, CancellationToken cancellationToken) {
        return await _roleWithFunctionsService.GetRoleAsync(id, cancellationToken);
    }
    
    [HttpPost]
    public async Task<int?> InsertRoleAsync([FromBody] RoleWithFunctionsInsertDto dto, CancellationToken cancellationToken) {
        return await _roleWithFunctionsService.InsertRoleAsync(dto, cancellationToken);
    }

    [HttpPut]
    public async Task<RoleWithFunctionsUpdateDto> UpdateRoleAsync(RoleWithFunctionsUpdateDto dto, CancellationToken cancellationToken) {
        return await _roleWithFunctionsService.UpdateRoleAsync(dto, cancellationToken);
    }

    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task DeleteRoleAsync(int id, CancellationToken cancellationToken) {
        await _roleWithFunctionsService.DeleteRoleAsync(id, cancellationToken);
    }

}