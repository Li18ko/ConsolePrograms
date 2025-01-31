using System.Security.Cryptography;
using System.Text;
using Log;
using MapsterMapper;

namespace WebInstallationOfFloorsApplication;

public class UserWithRolesService {
    private readonly Logger _logger;
    private readonly UserWithRolesRepository _userWithRolesRepository;
    private readonly IMapper _mapper;

    public UserWithRolesService(Logger logger, UserWithRolesRepository userWithRolesRepository, IMapper mapper) {
        _logger = logger;
        _userWithRolesRepository = userWithRolesRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserWithRolesGetDto>> GetAllUsersAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка пользователей");
        var users = await _userWithRolesRepository.GetAllUsersAsync(cancellationToken);
        _logger.Debug($"Найдено пользователей: {users.Count()}");
        
        return users.Select(user => _mapper.Map<UserWithRolesGetDto>(user)).ToList();
    }

    public async Task<UserWithRolesGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0) {
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение пользователя по id = {id}");
        var user = await _userWithRolesRepository.GetUserAsync(id, cancellationToken);

        if (user == null) {
            throw new Exception($"Пользователь с id = {id} не найден");
        }
        
        logDebugActionSuccessful($"найден c id = {id}");

        return _mapper.Map<UserWithRolesGetDto>(user);
    }

    public async Task<int?> InsertUserAsync(UserWithRolesInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return null;
        }
        
        logDebugRequestSuccessful("добавление нового пользователя");
        
        var user = _mapper.Map<User>(dto);
        
        foreach (var userRole in user.UserRoles) {
            userRole.User = user; 
        }
        
        var insertUser = await _userWithRolesRepository.InsertUserAsync(user, cancellationToken);
        logDebugActionSuccessful("добавлен");
        return insertUser;
    }

    public async Task<UserWithRolesUpdateDto> UpdateUserAsync(UserWithRolesUpdateDto dto, CancellationToken cancellationToken) {
        if (dto == null || dto.Id <= 0) {
            _logger.Warning("Некорректные данные пользователя в запросе на обновление");
            return null;
        }
        
        logDebugRequestSuccessful($"обновление данных о пользователе c id = {dto.Id}");
        var updatedUser = await _userWithRolesRepository.GetUserAsync(dto.Id, cancellationToken);
        if (updatedUser == null) {
            throw new Exception($"Пользователь с id = {dto.Id} не найден");
        }
        
        updatedUser = _mapper.Map(dto, updatedUser);
        
        foreach (var userRole in updatedUser.UserRoles) {
            userRole.User = updatedUser; 
        }
        
        await _userWithRolesRepository.UpdateUserAsync(updatedUser, cancellationToken);
        logDebugActionSuccessful($"найден c id = {dto.Id}");
        
        return _mapper.Map<UserWithRolesUpdateDto>(updatedUser);
    }
    
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о пользователе c id = {id}");
        var deleteUser = await _userWithRolesRepository.GetUserAsync(id, cancellationToken);
        if (deleteUser == null) {
            throw new Exception($"Пользователь с id = {id} не найден");
        }
            
        await _userWithRolesRepository.DeleteUserAsync(id, cancellationToken);
        logDebugActionSuccessful($"удален c id = {id}");
    }
    
    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Пользователь успешно " + action);
    }

}