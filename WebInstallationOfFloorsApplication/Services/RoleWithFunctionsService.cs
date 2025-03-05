using Log;
using MapsterMapper;

namespace WebInstallationOfFloorsApplication;

public class RoleWithFunctionsService {
    private readonly Logger _logger;
    private readonly RoleWithFunctionsRepository _roleWithFunctionsRepository;
    private readonly IMapper _mapper;

    public RoleWithFunctionsService(Logger logger, RoleWithFunctionsRepository roleWithFunctionsRepository, IMapper mapper) {
        _logger = logger;
        _roleWithFunctionsRepository = roleWithFunctionsRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<RoleWithFunctionsGetDto>> GetAllRolesAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка ролей и функций");
        var roles = await _roleWithFunctionsRepository.GetAllRolesAsync(cancellationToken);
        _logger.Debug($"Найдено ролей: {roles.Count()}");
        
        var rolesDto = roles.Select(role => _mapper.Map<RoleWithFunctionsGetDto>(role)).ToList();
        
        return rolesDto;
    }
    
    public async Task<RoleWithFunctionsGetDto> GetRoleAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение роли по id = {id}");
        var role = await _roleWithFunctionsRepository.GetRoleAsync(id, cancellationToken);

        if (role == null) {
            throw new Exception($"Роль с id = {id} не найдена");
        }
        logDebugActionSuccessful($"найдена c id = {id}");
        
        var roleDto = _mapper.Map<RoleWithFunctionsGetDto>(role);
        
        return roleDto;
    }
    
    public async Task<int?> InsertRoleAsync(RoleWithFunctionsInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные роли в запросе на добавление");
            return null;
        }
        
        logDebugRequestSuccessful("добавление новой роли");
        
        var role = _mapper.Map<Role>(dto);
        
        var insertRole = await _roleWithFunctionsRepository.InsertRoleAsync(role, cancellationToken);
        logDebugActionSuccessful("добавлена");
        return insertRole;
    }
    
    public async Task<RoleWithFunctionsUpdateDto> UpdateRoleAsync(RoleWithFunctionsUpdateDto dto, CancellationToken cancellationToken) {
        if (dto == null || dto.Id <= 0) {
            _logger.Warning("Некорректные данные роли в запросе на обновление");
            return null;
        }
        
        logDebugRequestSuccessful($"обновление данных о роль c id = {dto.Id}");
        var updatedRole = await _roleWithFunctionsRepository.GetRoleAsync(dto.Id, cancellationToken);
        if (updatedRole == null) {
            throw new Exception($"Роль с id = {dto.Id} не найдена");
        }
        
        updatedRole = _mapper.Map(dto, updatedRole);
        
        await _roleWithFunctionsRepository.UpdateRoleAsync(updatedRole, cancellationToken);
        logDebugActionSuccessful($"найден c id = {dto.Id}");
        
        return _mapper.Map<RoleWithFunctionsUpdateDto>(updatedRole);
    }
    
    public async System.Threading.Tasks.Task DeleteRoleAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о роли c id = {id}");
        var deleteRole = await _roleWithFunctionsRepository.GetRoleAsync(id, cancellationToken);
        if (deleteRole == null) {
            throw new Exception($"Роль с id = {id} не найдена");
        }
        
        bool isInUse = await _roleWithFunctionsRepository.IsRoleInUseAsync(id, cancellationToken);
        if (isInUse) {
            throw new Exception($"Роль используется и не может быть удалена");
        }
            
        await _roleWithFunctionsRepository.DeleteRoleAsync(id, cancellationToken);
        logDebugActionSuccessful($"удален c id = {id}");
    }
    
    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Задача успешно " + action);
    }
}