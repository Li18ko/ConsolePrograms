using Dapper;
using Npgsql;
using UserRepository.Model;

namespace UserRepository {
    public class UserRepositoryDapper: IUserRepository {
        private readonly string _connectionString;

        public UserRepositoryDapper(string connectionString) {
            _connectionString = connectionString;
        }
        
        private NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(_connectionString);
        }
        
        
        public IEnumerable<User> GetList() {
            try {
                using (var connection = GetConnection()) {
                    connection.Open();
                    
                    string query = "SELECT * FROM Users";
                    return connection.Query<User>(query);
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при получении списка пользователей: {e.Message}");
                return new List<User>();
            }
            
        }

        public IEnumerable<User> Get(int id) {
            try{
                using (var connection = GetConnection()) {
                    connection.Open();
                    string query = "SELECT * FROM Users WHERE id = @Id";
                    return connection.Query<User>(query, new { Id = id });
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при получении пользователя по id: {e.Message}");
                return new List<User>();
            }
        }

        public void Update(User user) {
            try{
                using (var connection = GetConnection()) {
                    connection.Open();
                    string checkQuery = "SELECT * FROM Users WHERE id = @Id";
                    var exists = connection.ExecuteScalar<int>(checkQuery, new { id = user.Id }) > 0;
                    
                    if (!exists) {
                        Console.WriteLine($"Пользователь с id {user.Id} не найден. Обновление не выполнено.");
                        return;
                    }
                    
                    string query = "UPDATE Users SET email = @Email, login = @Login, name = @Name, phone = @Phone, note = @Note WHERE id = @Id";
                    connection.Execute(query, user);
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при обновлении данных пользователя: {e.Message}");
            }
        }

        public void Delete(int id) {
            try{
                using (var connection = GetConnection()) {
                    connection.Open();
                    string query = "DELETE FROM Users WHERE id = @Id";
                    connection.Execute(query, new { Id = id });
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при удалении пользователя: {e.Message}");
            }
        }

        public void Insert(User user) {
            try{
                using (var connection = GetConnection()) {
                    connection.Open();
                    string query = "INSERT INTO Users (email, login, name, phone, note) VALUES (@Email, @Login, @Name, @Phone, @Note)";
                    connection.Execute(query, user);
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при добавлении пользователя: {e.Message}");
            }
        }
    }
}
