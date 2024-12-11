using UserRepository.Model;

namespace UserRepository {
    public interface IUserRepository {
        IEnumerable<User> GetList();
        IEnumerable<User> Get(int id);
        void Update(User user);
        void Delete(int id);
        void Insert(User user);
    }
}
