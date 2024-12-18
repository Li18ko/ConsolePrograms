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

    public IEnumerable<User> GetUsers(CancellationToken cancellationToken) {
        _logger.Info("Получен запрос на получение списка пользователей");
        var users = _userRepository.GetList(cancellationToken);
        _logger.Info($"Найдено пользователей: {users.Count()}");
        return users;
    }

    public User Get(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return null;
        }
        _logger.Info($"Получен запрос на получение пользователя по id = {id}");
        var user = _userRepository.Get(id, cancellationToken);

        if (user == null) {
            _logger.Warning($"Пользователь с id = {id} не найден");
            return null;
        }
        _logger.Info($"Пользователь c {id} найден");
        return user;
    }
    
    public string Update([FromBody] User user, CancellationToken cancellationToken) {
        if (user == null || user.Id <= 0) {
            _logger.Warning("Некорректные данные пользователя в запросе на обновление");
            return "Некорректные данные пользователя";
        }
        _logger.Info($"Получен запрос на обновление данных о пользователе c id = {user.Id}");
        var updatedUser = _userRepository.Get(user.Id, cancellationToken);
        if (updatedUser == null) {
            _logger.Warning($"Пользователь с id = {user.Id} не найден для обновления");
            return $"Пользователь с id = {user.Id} не найден для обновления";
        }
        _userRepository.Update(user, cancellationToken);
        _logger.Info($"Пользователь c id = {user.Id} найден");
        return $"Данные о пользователе c id = {user.Id} успешно обновлены";
    }

    public string Delete(int id, CancellationToken cancellationToken) {
        if (id <= 0){
            _logger.Warning("Некорректный id");
            return "Некорректный ID";
        }
        _logger.Info($"Получен запрос на удаление данных о пользователе c id = {id}");
        var deleteUser = _userRepository.Get(id, cancellationToken);
        if (deleteUser == null) {
            _logger.Warning($"Пользователь с id = {id} не найден для удаления");
            return $"Пользователь с id = {id} не найден для удаления";
        }
            
        _userRepository.Delete(id, cancellationToken);
        _logger.Info($"Пользователь c id = {id} успешно удален");
        return $"Пользователь c id = {id} успешно удалён";
    }
    
    public string Insert([FromBody] UserWithoutId user, CancellationToken cancellationToken) {
        if (user == null) {
            _logger.Warning("Некорректные данные пользователя в запросе на добавление");
            return "Некорректные данные пользователя в запросе на добавление";
        }
        _logger.Info($"Получен запрос на добавление нового пользователя");
        _userRepository.Insert(user, cancellationToken);
        _logger.Info($"Пользователь успешно добавлен");
        return $"Пользователь успешно добавлен";
    }

}