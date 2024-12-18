using System.Data.Common;
using Dapper;
using ListRepositories.ConnectionFactory;
using ListRepositories.Model;


namespace ListRepositories {
    public class UserRepository: IUserRepository {
        private readonly IConnectionFactory _connection;
        private IUserRepository _userRepositoryImplementation;

        public UserRepository(IConnectionFactory connection) {
            _connection = connection;
        }
        
        public IEnumerable<User> GetList(CancellationToken cancellationToken) {
            string query = "SELECT * FROM Users";
            return ConnectionTemplate(connection => connection.Query<User>(query), cancellationToken);
        }

        public User Get(int id, CancellationToken cancellationToken) {
            string query = "SELECT * FROM Users WHERE id = @Id";
            return ConnectionTemplate(connection => connection.QuerySingleOrDefault<User>(query, new { Id = id }), cancellationToken);
        }

        public void Update(User user, CancellationToken cancellationToken) {
            string query = "UPDATE Users SET email = @Email, login = @Login, name = @Name, phone = @Phone, note = @Note WHERE id = @Id";
            ConnectionTemplate(connection => connection.Execute(query, user), cancellationToken);
        }

        public void Delete(int id, CancellationToken cancellationToken) {
            string query = "DELETE FROM Users WHERE id = @Id";
            ConnectionTemplate(connection => connection.Execute(query, new { Id = id }), cancellationToken);
        }

        public void Insert(UserWithoutId user, CancellationToken cancellationToken) {
            string query = "INSERT INTO Users (email, login, name, phone, note) VALUES (@Email, @Login, @Name, @Phone, @Note)";
            ConnectionTemplate(connection => connection.Execute(query, user), cancellationToken);
        }

        private T ConnectionTemplate<T>(Func<DbConnection, T> query, CancellationToken cancellationToken) {
            using (var connection = _connection.GetConnection()) {
                connection.Open();
                cancellationToken.ThrowIfCancellationRequested();
                return query(connection);
            }
        }
    }
}
