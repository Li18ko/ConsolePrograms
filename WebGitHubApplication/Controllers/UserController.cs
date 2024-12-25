using Microsoft.AspNetCore.Mvc;
using ListRepositories;
using ListRepositories.Model;
using ServiceListRepositories;

namespace WebGitHubApplication {
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController: ControllerBase {
        private readonly UserService _userService;
        
        public UserController(UserService userService) {
            _userService = userService;
        }
        
        [HttpGet("List")]
        public async Task<IEnumerable<User>> GetListAsync(CancellationToken cancellationToken) {
            return await _userService.GetUsersAsync(cancellationToken);
        }
        
        [HttpGet("{id}")]
        public async Task<User> GetAsync(int id, CancellationToken cancellationToken) {
            return await _userService.GetAsync(id, cancellationToken);;
        }
        
        [HttpPut]
        public async Task<User> UpdateAsync([FromBody] User user, CancellationToken cancellationToken) {
            return await _userService.UpdateAsync(user, cancellationToken);
        }
        
        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id, CancellationToken cancellationToken) {
            await _userService.DeleteAsync(id, cancellationToken);
        }
        
        [HttpPost]
        public async Task<int?> InsertAsync([FromBody] UserWithoutId user, CancellationToken cancellationToken) {
            return await _userService.InsertAsync(user, cancellationToken);
        }
        
    
    }
}   
