using System.Security.Cryptography;
using System.Text;
using Log;

namespace WebInstallationOfFloorsApplication;

public class UserWithRolesService {
    private readonly Logger _logger;
    private readonly UserWithRolesRepository _userWithRolesRepository;

    public UserWithRolesService(Logger logger, UserWithRolesRepository userWithRolesRepository) {
        _logger = logger;
        _userWithRolesRepository = userWithRolesRepository;
    }

    public async Task<IEnumerable<UserWithRolesGetDto>> GetAllUsersAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка пользователей");
        var users = await _userWithRolesRepository.GetAllUsersAsync(cancellationToken);
        _logger.Debug($"Найдено пользователей: {users.Count()}");
        
        var userDtos = users.Select(user => new UserWithRolesGetDto {
            Id = user.Id,
            Name = user.Name,
            Login = user.Login,
            ChatId = user.ChatId,
            Roles = user.UserRoles.Select(ur => ur.RoleId).ToList() 
        }).ToList();

        return userDtos;
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
        
        var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        
        logDebugActionSuccessful($"найден c id = {id}");
        var userWithRolesDto = new UserWithRolesGetDto {
            Id = user.Id,
            Name = user.Name,
            Login = user.Login,
            ChatId = user.ChatId,
            Roles = roleIds 
        };

        return userWithRolesDto;
    }

    public async Task<int?> InsertUserAsync(UserWithRolesInsertDto dto, CancellationToken cancellationToken) {
        if (dto == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return null;
        }
        
        logDebugRequestSuccessful("добавление нового пользователя");
        
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
        
        updatedUser.Name = dto.Name;
        updatedUser.Login = dto.Login;
        updatedUser.ChatId = dto.ChatId;

        if (!string.IsNullOrEmpty(dto.Password)) {
            updatedUser.Password = Convert.ToHexString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(dto.Password)));
        }
        
        if (dto.RoleIds != null) {
            updatedUser.UserRoles = dto.RoleIds.Select(roleId => new UserRole { RoleId = roleId, User = updatedUser }).ToList();
        }
        
        await _userWithRolesRepository.UpdateUserAsync(updatedUser, cancellationToken);
        logDebugActionSuccessful($"найден c id = {dto.Id}");

        return new UserWithRolesUpdateDto {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Login = updatedUser.Login,
            ChatId = updatedUser.ChatId,
            RoleIds = updatedUser.UserRoles.Select(ur => ur.RoleId).ToList(),
            Password = "******"
        };
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