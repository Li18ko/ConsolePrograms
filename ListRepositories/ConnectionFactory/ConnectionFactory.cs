using Npgsql;

namespace ListRepositories.ConnectionFactory {
    public class ConnectionFactory:IConnectionFactory {
        private readonly string _connection;

        public ConnectionFactory(string connection) {
            _connection = connection;
        }

        public NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(_connection);
        }

    }
}
