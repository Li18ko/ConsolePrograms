using ListRepositories.Model;

namespace ListRepositories {
    public interface IUserRepository {
        IEnumerable<User> GetList(CancellationToken cancellationToken);
        User Get(int id, CancellationToken cancellationToken);
        void Update(User user, CancellationToken cancellationToken);
        void Delete(int id, CancellationToken cancellationToken);
        void Insert(UserWithoutId user, CancellationToken cancellationToken);
    }
}
