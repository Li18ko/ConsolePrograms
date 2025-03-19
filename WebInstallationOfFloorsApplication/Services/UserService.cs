using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Log;
using MapsterMapper;

namespace WebInstallationOfFloorsApplication;

public class UserService {
    private readonly Logger _logger;
    private readonly UserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(Logger logger, UserRepository userRepository, IMapper mapper) {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserGetDto>> GetAllUsersAsync(string sort, string order, IEnumerable<int>? filter, 
        string search, int skip, int take,
        CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка пользователей");
        var (users, totalCount) = await _userRepository.GetAllUsersAsync(sort, order, filter, search, skip, take, cancellationToken);
        _logger.Debug($"Найдено пользователей: {users.Count()}");
        
        var userDtos = users.Select(user => _mapper.Map<UserGetDto>(user)).ToList();
        
        return new PagedResult<UserGetDto>{
            Count = totalCount,
            Items = userDtos.ToList()
        };
    }

    public async Task<UserGetDto> GetUserAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0) {
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение пользователя по id = {id}");
        var user = await _userRepository.GetUserAsync(id, cancellationToken);

        if (user == null) {
            throw new Exception($"Пользователь с id = {id} не найден");
        }
        
        logDebugActionSuccessful($"найден c id = {id}");

        return _mapper.Map<UserGetDto>(user);
    }
        
    public async Task<bool> IsLoginTakenAsync(int? id, string login, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(login)) {
            _logger.Warning("Некорректный логин");
            return false;
        }
        
        if (id.HasValue && id <= 0) {
            _logger.Warning("Некорректный id");
            return false;
        }
        
        logDebugRequestSuccessful($"проверку существования логина = {login}" + (id.HasValue ? $" не у аккаунта с id = {id}" : ""));
        var existingLogin = await _userRepository.IsLoginTakenAsync(id, login, cancellationToken);
        logDebugActionSuccessful($"найден c логином = {login}");

        return existingLogin;
    }
    
    public async Task<bool> IsEmailTakenAsync(int? id, string email, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(email)) {
            _logger.Warning("Некорректная почта");
            return false;
        }
        
        if (id.HasValue && id <= 0) {
            _logger.Warning("Некорректный id");
            return false;
        }
        
        logDebugRequestSuccessful($"проверку существования почты = {email}" + (id.HasValue ? $" не у аккаунта с id = {id}" : ""));
        var existingEmail = await _userRepository.IsEmailTakenAsync(id, email, cancellationToken);
        logDebugActionSuccessful($"найден c почтой = {email}");

        return existingEmail;
    }
    

    public async Task<int?> InsertUserAsync(UserInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return null;
        }
        
        var existingLogin = await _userRepository.IsLoginTakenAsync(null, dto.Login, cancellationToken);
        if (existingLogin) {
            _logger.Warning("Логин уже существует");
            throw new Exception($"Логин уже существует");
        }
        
        var existingEmail = await _userRepository.IsEmailTakenAsync(null, dto.Email, cancellationToken);
        if (existingEmail) {
            _logger.Warning("Почта уже существует");
            throw new Exception($"Почта уже существует");
        }
        
        logDebugRequestSuccessful("добавление нового пользователя");
        
        var user = _mapper.Map<User>(dto);
        
        foreach (var userRole in user.UserRoles) {
            userRole.User = user; 
        }
        
        var insertUser = await _userRepository.InsertUserAsync(user, cancellationToken);
        logDebugActionSuccessful("добавлен");
        return insertUser;
    }

    public async Task<UserUpdateDto> UpdateUserAsync(UserUpdateDto dto, CancellationToken cancellationToken) {
        if (dto == null || dto.Id <= 0) {
            _logger.Warning("Некорректные данные пользователя в запросе на обновление");
            return null;
        }
        
        var existingLogin = await _userRepository.IsLoginTakenAsync(dto.Id, dto.Login, cancellationToken);
        if (existingLogin) {
            _logger.Warning("Логин уже существует");
            throw new Exception($"Логин уже существует");
        }
        
        var existingEmail = await _userRepository.IsEmailTakenAsync(dto.Id, dto.Email, cancellationToken);
        if (existingEmail) {
            _logger.Warning("Почта уже существует");
            throw new Exception($"Почта уже существует");
        }
        
        logDebugRequestSuccessful($"обновление данных о пользователе c id = {dto.Id}");
        var updatedUser = await _userRepository.GetUserAsync(dto.Id, cancellationToken);
        if (updatedUser == null) {
            throw new Exception($"Пользователь с id = {dto.Id} не найден");
        }
        
        updatedUser = _mapper.Map(dto, updatedUser);
        
        if (string.IsNullOrWhiteSpace(dto.Password)) {
            updatedUser.Password = await _userRepository.GetUserPasswordAsync(dto.Id, cancellationToken);
        }
        
        foreach (var userRole in updatedUser.UserRoles) {
            userRole.User = updatedUser; 
        }
        
        await _userRepository.UpdateUserAsync(updatedUser, cancellationToken);
        logDebugActionSuccessful($"найден c id = {dto.Id}");
        
        return _mapper.Map<UserUpdateDto>(updatedUser);
    }
    
    public async System.Threading.Tasks.Task DeleteUserAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о пользователе c id = {id}");
        var deleteUser = await _userRepository.GetUserAsync(id, cancellationToken);
        if (deleteUser == null) {
            throw new Exception($"Пользователь с id = {id} не найден");
        }
            
        await _userRepository.DeleteUserAsync(id, cancellationToken);
        logDebugActionSuccessful($"удален c id = {id}");
    }
    
    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Пользователь успешно " + action);
    }
}