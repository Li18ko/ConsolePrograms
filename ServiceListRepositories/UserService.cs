using ListRepositories;
using ListRepositories.Model;
using Log;
using Microsoft.AspNetCore.Mvc;

namespace ServiceListRepositories;

public class UserService {
    private readonly IUserRepository _userRepository;
    private readonly Logger _logger;

    public UserService(IUserRepository userRepository, Logger logger){
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken) {
        logDebugRequestSuccessful("получение списка пользователей");
        var users = await _userRepository.GetListAsync(cancellationToken);
        _logger.Debug($"Найдено пользователей: {users.Count()}");
        return users;
    }

    public async Task<User> GetAsync(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return null;
        }
        logDebugRequestSuccessful($"получение пользователя по id = {id}");
        var user = await _userRepository.GetAsync(id, cancellationToken);

        if (user == null) {
            throw new Exception($"Пользователь с id = {id} не найден.");
        }
        logDebugActionSuccessful($"найден c id = {id}");
        return user;
    }
    
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken) {
        if (user == null || user.Id <= 0) {
            _logger.Warning("Некорректные данные пользователя в запросе на обновление");
            return null;
        }
        
        var roleExists = await _userRepository.RoleExistsAsync(user.RoleId, cancellationToken);
        if (roleExists == 0) {
            throw new Exception($"Роль с id = {user.RoleId} не найдена");
        }
        
        logDebugRequestSuccessful($"обновление данных о пользователе c id = {user.Id}");
        var updatedUser = await _userRepository.GetAsync(user.Id, cancellationToken);
        if (updatedUser == null) {
            throw new Exception($"Пользователь с id = {user.Id} не найден");
        }
        
        await _userRepository.UpdateAsync(user, cancellationToken);
        logDebugActionSuccessful($"найден c id = {user.Id}");
        
        var getUpdatedUser = await _userRepository.GetAsync(user.Id, cancellationToken);
        
        return getUpdatedUser;
    }

    public async Task DeleteAsync (int id, CancellationToken cancellationToken) {
        if (id <= 0){
            throw new Exception("id должен быть больше нуля");
        }
        logDebugRequestSuccessful($"удаление данных о пользователе c id = {id}");
        var deleteUser = await _userRepository.GetAsync(id, cancellationToken);
        if (deleteUser == null) {
            throw new Exception($"Пользователь с id = {id} не найден.");
        }
            
        await _userRepository.DeleteAsync(id, cancellationToken);
        logDebugActionSuccessful($"удален c id = {id}");
    }
    
    public async Task<int?> InsertAsync(UserWithoutId user, CancellationToken cancellationToken) {
        if (user == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return null;
        }
        
        var roleExists = await _userRepository.RoleExistsAsync(user.RoleId, cancellationToken);
        if (roleExists == 0) {
            throw new Exception($"Роль с id = {user.RoleId} не найдена");
        }
        
        logDebugRequestSuccessful("добавление нового пользователя");
        var insertUser = await _userRepository.InsertAsync(user, cancellationToken);
        logDebugActionSuccessful("добавлен");
        return insertUser;
    }

    private void logDebugRequestSuccessful(string action) {
        _logger.Debug("Получен запрос на " + action);
    }
    
    private void logDebugActionSuccessful(string action) {
        _logger.Debug("Пользователь успешно " + action);
    }

} 