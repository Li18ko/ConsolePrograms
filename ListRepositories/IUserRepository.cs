using ListRepositories.Model;

namespace ListRepositories {
    public interface IUserRepository {
        Task<IEnumerable<User>> GetListAsync(CancellationToken cancellationToken);
        Task<User?> GetAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<int> InsertAsync(UserWithoutId user, CancellationToken cancellationToken);

        Task<int> RoleExistsAsync(int id, CancellationToken cancellationToken);
    }
}
