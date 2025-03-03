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

    public async Task<IEnumerable<UserWithRolesGetDto>> GetAllUsersAsync(string sort, CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка пользователей");
        var users = await _userWithRolesRepository.GetAllUsersAsync(sort, cancellationToken);
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
    
    public async Task<bool> IsLoginTakenByOtherUserAsync(int id, string login, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(login)) {
            _logger.Warning("Некорректный логин");
            return false;
        }
        
        if (id <= 0) {
            _logger.Warning("Некорректный id");
            return false;
        }
        
        logDebugRequestSuccessful($"проверку существования логина = {login} не у аккаунта с id = {id}");
        var existingLogin = await _userWithRolesRepository.IsLoginTakenByOtherUserAsync(id, login, cancellationToken);
        logDebugActionSuccessful($"найден c логином = {login}");

        return existingLogin;
    }
        
    public async Task<bool> IsLoginTakenAsync(string login, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(login)) {
            _logger.Warning("Некорректный логин");
            return false;
        }
        logDebugRequestSuccessful($"проверку существования логина = {login}");
        var existingLogin = await _userWithRolesRepository.IsLoginTakenAsync(login, cancellationToken);
        logDebugActionSuccessful($"найден c логином = {login}");

        return existingLogin;
    }
    
    public async Task<bool> IsEmailTakenByOtherUserAsync(int id, string email, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(email)) {
            _logger.Warning("Некорректная почта");
            return false;
        }
        
        if (id <= 0) {
            _logger.Warning("Некорректный id");
            return false;
        }
        
        logDebugRequestSuccessful($"проверку существования почты = {email} не у аккаунта с id = {id}");
        var existingEmail = await _userWithRolesRepository.IsEmailTakenByOtherUserAsync(id, email, cancellationToken);
        logDebugActionSuccessful($"найден c почтой = {email}");

        return existingEmail;
    }
        
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(email)) {
            _logger.Warning("Некорректная почта");
            return false;
        }
        logDebugRequestSuccessful($"проверку существования почты = {email}");
        var existingEmail = await _userWithRolesRepository.IsEmailTakenAsync(email, cancellationToken);
        logDebugActionSuccessful($"найден c почтой = {email}");

        return existingEmail;
    }

    public async Task<int?> InsertUserAsync(UserWithRolesInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return null;
        }
        
        var existingLogin = await _userWithRolesRepository.IsLoginTakenAsync(dto.Login, cancellationToken);
        if ( existingLogin) {
            _logger.Warning("Логин уже существует");
            throw new Exception($"Логин уже существует");
        }
        
        var existingEmail = await _userWithRolesRepository.IsEmailTakenAsync(dto.Email, cancellationToken);
        if (existingEmail) {
            _logger.Warning("Почта уже существует");
            throw new Exception($"Почта уже существует");
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
        
        var existingLogin = await _userWithRolesRepository.IsLoginTakenByOtherUserAsync(dto.Id, dto.Login, cancellationToken);
        if (existingLogin) {
            _logger.Warning("Логин уже существует");
            throw new Exception($"Логин уже существует");
        }
        
        var existingEmail = await _userWithRolesRepository.IsEmailTakenByOtherUserAsync(dto.Id, dto.Email, cancellationToken);
        if (existingEmail) {
            _logger.Warning("Почта уже существует");
            throw new Exception($"Почта уже существует");
        }
        
        logDebugRequestSuccessful($"обновление данных о пользователе c id = {dto.Id}");
        var updatedUser = await _userWithRolesRepository.GetUserAsync(dto.Id, cancellationToken);
        if (updatedUser == null) {
            throw new Exception($"Пользователь с id = {dto.Id} не найден");
        }
        
        updatedUser = _mapper.Map(dto, updatedUser);
        
        if (string.IsNullOrWhiteSpace(dto.Password)) {
            updatedUser.Password = await _userWithRolesRepository.GetUserPasswordAsync(dto.Id, cancellationToken);
        }
        
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