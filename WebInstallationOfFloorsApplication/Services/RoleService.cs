using Log;
using MapsterMapper;

namespace WebInstallationOfFloorsApplication;

public class RoleService {
    private readonly Logger _logger;
    private readonly RoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(Logger logger, RoleRepository roleRepository, IMapper mapper) {
        _logger = logger;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<RoleGetDto>> GetAllRolesAsync(IEnumerable<bool>? status, 
        int skip, int take,CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка ролей и функций");
        var (roles, totalCount) = await _roleRepository.GetAllRolesAsync(status, skip, take, cancellationToken);
        _logger.Debug($"Найдено ролей: {roles.Count()}");
        
        var rolesDto = roles.Select(role => _mapper.Map<RoleGetDto>(role)).ToList();
        
        return new PagedResult<RoleGetDto>{
            Count = totalCount,
            Items = rolesDto
        };
    }
    
    public async Task<IEnumerable<RoleGetDto>> GetAllRolesWithoutSortingAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка ролей и функций");
        var roles = await _roleRepository.GetAllRolesWithoutSortingAsync(cancellationToken);
        _logger.Debug($"Найдено ролей: {roles.Count()}");
        
        var rolesDto = roles.Select(role => _mapper.Map<RoleGetDto>(role)).ToList();

        return rolesDto;
    }
    
    public async Task<RoleGetDto> GetRoleAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение роли по id = {id}");
        var role = await _roleRepository.GetRoleAsync(id, cancellationToken);

        if (role == null) {
            throw new Exception($"Роль с id = {id} не найдена");
        }
        logDebugActionSuccessful($"найдена c id = {id}");
        
        var roleDto = _mapper.Map<RoleGetDto>(role);
        
        return roleDto;
    }
    
    public async Task<int?> InsertRoleAsync(RoleInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные роли в запросе на добавление");
            return null;
        }
        
        logDebugRequestSuccessful("добавление новой роли");
        
        var role = _mapper.Map<Role>(dto);
        
        var insertRole = await _roleRepository.InsertRoleAsync(role, cancellationToken);
        logDebugActionSuccessful("добавлена");
        return insertRole;
    }
    
    public async Task<RoleUpdateDto> UpdateRoleAsync(RoleUpdateDto dto, CancellationToken cancellationToken) {
        if (dto == null || dto.Id <= 0) {
            _logger.Warning("Некорректные данные роли в запросе на обновление");
            return null;
        }
        
        logDebugRequestSuccessful($"обновление данных о роль c id = {dto.Id}");
        var updatedRole = await _roleRepository.GetRoleAsync(dto.Id, cancellationToken);
        if (updatedRole == null) {
            throw new Exception($"Роль с id = {dto.Id} не найдена");
        }
        
        updatedRole = _mapper.Map(dto, updatedRole);
        
        await _roleRepository.UpdateRoleAsync(updatedRole, cancellationToken);
        logDebugActionSuccessful($"найден c id = {dto.Id}");
        
        return _mapper.Map<RoleUpdateDto>(updatedRole);
    }
    
    public async System.Threading.Tasks.Task DeleteRoleAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о роли c id = {id}");
        var deleteRole = await _roleRepository.GetRoleAsync(id, cancellationToken);
        if (deleteRole == null) {
            throw new Exception($"Роль с id = {id} не найдена");
        }
        
        bool isInUse = await _roleRepository.IsRoleInUseAsync(id, cancellationToken);
        if (isInUse) {
            throw new Exception($"Роль используется и не может быть удалена");
        }
            
        await _roleRepository.DeleteRoleAsync(id, cancellationToken);
        logDebugActionSuccessful($"удален c id = {id}");
    }
    
    public async Task<IEnumerable<Function>> GetAllFunctionsAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка функций");
        var functions = await _roleRepository.GetAllFunctionsAsync(cancellationToken);
        _logger.Debug($"Найдено функций: {functions.Count()}");
        
        return functions;
    }
    
    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Задача успешно " + action);
    }
}