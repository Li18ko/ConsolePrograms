using Microsoft.AspNetCore.Mvc;
using ListRepositories;
using ListRepositories.Model;
using ServiceListRepositories;

namespace WebGitHubApplication {
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase {
        private readonly UserService _userService;
        private readonly IUserRepository _userRepository;
        
        public UserController(UserService userService) {
            _userService = userService;
        }
        
        [HttpGet("userList")]
        public IEnumerable<User> GetList(CancellationToken cancellationToken) {
            return _userService.GetUsers(cancellationToken);
        }
        
        [HttpGet("{id}")]
        public User Get(int id, CancellationToken cancellationToken) {
            return _userService.Get(id, cancellationToken);;
        }
        
        [HttpPut]
        public string Update([FromBody] User user, CancellationToken cancellationToken) {
            return _userService.Update(user, cancellationToken);
        }
        
        [HttpDelete("{id}")]
        public string Delete(int id, CancellationToken cancellationToken) {
            return _userService.Delete(id, cancellationToken);
        }
        
        [HttpPost]
        public string Insert([FromBody] UserWithoutId user, CancellationToken cancellationToken) {
            return _userService.Insert(user, cancellationToken);
        }
        
    
    }
}
