using System.Data.Common;
using Dapper;
using ListRepositories.ConnectionFactory;
using ListRepositories.Model;


namespace ListRepositories {
    public class UserRepository: IUserRepository {
        private readonly IConnectionFactory _connection;

        public UserRepository(IConnectionFactory connection) {
            _connection = connection;
        }
        
        public async Task<IEnumerable<User>> GetListAsync (CancellationToken token) {
            string query = "SELECT * FROM Users JOIN Roles ON Users.roleId = Roles.Id";
            return await ConnectionTemplateAsync(async(connection, ct) => {
                var result = await connection.QueryAsync<User, Role, User>(
                    new CommandDefinition(query, cancellationToken: ct), 
                    (user, role) => {
                        user.Role = role;
                        user.RoleId = role.Id;
                        return user;
                    },
                    splitOn: "RoleId");
                return result;
            }, token);
        }

        public async Task<User?> GetAsync(int id, CancellationToken token) {
            string query = "SELECT * FROM Users JOIN Roles ON Users.roleId = Roles.Id WHERE Users.Id =@Id";
            return await ConnectionTemplateAsync(async (connection, ct) => {
                var userDictionary = new Dictionary<int, User>();
                 await connection.QueryAsync<User, Role, User>(
                    new CommandDefinition(query, new { Id = id }, cancellationToken: ct), 
                    (user, role) => {
                        if (!userDictionary.TryGetValue(user.Id, out var userEntry)){
                            userEntry = user;
                            userEntry.RoleId = role.Id;
                            userEntry.Role = role;
                            userDictionary.Add(userEntry.Id, userEntry);
                        }
                        return userEntry;
                    },
                    splitOn: "RoleId");
                 return userDictionary.Values.SingleOrDefault();
            }, token);
        } 

        public async Task UpdateAsync(User user, CancellationToken token) {
            string query = "UPDATE Users SET email = @Email, login = @Login, name = @Name, phone = @Phone, note = @Note, roleId = @RoleId WHERE id = @Id";
            var parameters = new {
                Email = user.Email,
                Login = user.Login,
                Name = user.Name,
                Phone = user.Phone,
                Note = user.Note,
                RoleId = user.RoleId,
                Id = user.Id
            };
            await ConnectionTemplateAsync(async (connection, ct) => {
                await connection.ExecuteAsync(new CommandDefinition(query, parameters, cancellationToken: ct));
            }, token);
        } 

        public async Task DeleteAsync(int id, CancellationToken token) {
            string query = "DELETE FROM Users WHERE id = @Id";
            await ConnectionTemplateAsync(async (connection, ct) => 
            {
                await connection.ExecuteAsync(new CommandDefinition(query, new { Id = id }, cancellationToken: ct));
            }, token);
        }

        public async Task<int> InsertAsync(UserWithoutId user, CancellationToken token) {
            string query = "INSERT INTO Users (email, login, name, phone, note, roleid) VALUES (@Email, @Login, @Name, @Phone, @Note, @RoleId) RETURNING id;";
            int insertedId = 0;
            await ConnectionTemplateAsync(async (connection, ct) => {
                insertedId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, user, cancellationToken: ct));
            }, token);
            return insertedId;
        }

        private async Task<T> ConnectionTemplateAsync<T>(Func<DbConnection, CancellationToken, Task<T>> query, CancellationToken cancellationToken) {
            await using (var connection = _connection.GetConnection()) {
                await connection.OpenAsync(cancellationToken);
                return await query(connection, cancellationToken);
            }
        }
        
        private async Task ConnectionTemplateAsync(Func<DbConnection, CancellationToken, Task> query, CancellationToken cancellationToken) {
            await using var connection = _connection.GetConnection(); {
                await connection.OpenAsync(cancellationToken);
                await query(connection, cancellationToken);
            }
        }


        public async Task<int> RoleExistsAsync(int id, CancellationToken token) {
            string query = "SELECT COUNT(1) FROM Roles WHERE id = @Id";
            var roleExists = await ConnectionTemplateAsync(async (connection, ct) => {
                var result = await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, new { Id = id }, cancellationToken: ct));
                return result;
            }, token);
            return roleExists;
        }
    }
}
