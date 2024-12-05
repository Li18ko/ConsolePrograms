using Dapper;
using DatabaseTests.Model;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;

namespace DatabaseTests {
    public class DatabaseConnection{
        private static string connectionString = "Server=localhost; Port=3306; User ID=root; Password=; Database=DbTests";
    
        private static DatabaseConnection _instance;
    
        private DatabaseConnection() { }
        
        public static DatabaseConnection Instance {
            get {
                if (_instance == null) {
                    _instance = new DatabaseConnection();
                }
                return _instance;
            }
        }

        private MySqlConnection GetConnection() {
            return new MySqlConnection(connectionString); 
        }

        public void InsertUser(string _name, int _age) {
            if (_name.IsNullOrEmpty()) {
                throw new ArgumentNullException("Имя не может быть пустым или содержать только пробелы.", nameof(_name));
            }

            if (_age < 0) {
                throw new ArgumentException("Возраст должен быть положительным числом.", nameof(_age));
            }
            try {
                using (var connection = GetConnection()) {
                    connection.Open();
                    string query = "INSERT INTO Users (name, age) VALUES (@Name, @Age)";
                    connection.Execute(query, new { name = _name, age = _age });
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Ошибка при добавлении пользователя: {e.Message}");
            }
        }

        public IEnumerable<User> GetUsers() {
            try {
                using (var connection = GetConnection()) {
                    connection.Open();
                    string query = "SELECT * FROM Users";
                    return connection.Query<User>(query);
                }
            }
            catch(Exception e) {
                Console.WriteLine($"Ошибка при получении пользователей: {e.Message}");
                return new List<User>();
            }
            
        }
    
    }
}