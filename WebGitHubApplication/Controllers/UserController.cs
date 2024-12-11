using Microsoft.AspNetCore.Mvc;
using UserRepository;
using UserRepository.Model;

namespace WebGitHubApplication {
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase {
        private readonly Log.Logger _logger;
        private readonly IUserRepository _userRepository;
        
        public UserController(IUserRepository userRepository, Log.Logger logger) {
            _userRepository = userRepository;
            _logger = logger;
        }
        
        [HttpGet("userList")]
        public async Task<IActionResult> GetList() {
            _logger.Info("Получен запрос на получение списка пользователей.");
            var users = _userRepository.GetList();
            _logger.Info($"Найдено пользователей: {users.Count()}");
            return Ok(users);
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int id) {
            if (id <= 0){
                _logger.Warning("Некорректный id");
                return BadRequest(new { Error = "Некорректный ID пользователя." });
            }
            _logger.Info($"Получен запрос на получение пользователя по id = {id}.");
            var user = _userRepository.Get(id);

            if (user.Count() == 0) {
                _logger.Warning($"Пользователь с id = {id} не найден.");
                return NotFound(new { Error = $"Пользователь с id = {id} не найден." });
            }
            _logger.Info($"Пользователь c {id} найден");
            return Ok(user);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user) {
            if (user == null || user.Id <= 0) {
                _logger.Warning("Некорректные данные пользователя в запросе на обновление.");
                return BadRequest(new { Error = "Некорректные данные пользователя." });
            }
            _logger.Info($"Получен запрос на обновление данных о пользователе c id = {user.Id}.");
            var updatedUser = _userRepository.Get(user.Id);
            if (updatedUser.Count() == 0) {
                _logger.Warning($"Пользователь с id = {user.Id} не найден для обновления.");
                return NotFound(new { Error = $"Пользователь с id = {user.Id} не найден." });
            }
            _userRepository.Update(user);
            _logger.Info($"Пользователь c id = {user.Id} найден");
            return Ok(new { Message = $"Данные о пользователе c id = {user.Id} успешно обновлены." });
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(int id) {
            if (id <= 0){
                _logger.Warning("Некорректный id");
                return BadRequest(new { Error = "Некорректный ID пользователя." });
            }
            _logger.Info($"Получен запрос на удаление данных о пользователе c id = {id}.");
            var deleteUser = _userRepository.Get(id);
            if (deleteUser.Count() == 0) {
                _logger.Warning($"Пользователь с id = {id} не найден для удаления.");
                return NotFound(new { Error = $"Пользователь с id = {id} не найден." });
            }
            
            _userRepository.Delete(id);
            _logger.Info($"Пользователь c id = {id} успешно удален");
            return Ok(new { Message = $"Пользователь c id = {id} успешно удалён." });
        }
        
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] User user) {
            if (user == null) {
                _logger.Warning("Некорректные данные пользователя в запросе на добавление.");
                return BadRequest(new { Error = "Некорректные данные пользователя." });
            }
            
            user.Id = 0;
            
            _logger.Info($"Получен запрос на добавление нового пользователя c id = {user.Id}.");
            _userRepository.Insert(user);
            _logger.Info($"Пользователь c id = {user.Id} успешно добавлен");
            return Ok(new { Message = $"Пользователь c id = {user.Id} успешно добавлен." });
        }
        
    
    }
}
