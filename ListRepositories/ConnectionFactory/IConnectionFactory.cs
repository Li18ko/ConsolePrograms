using Npgsql;

namespace ListRepositories.ConnectionFactory {
    public interface IConnectionFactory {
        NpgsqlConnection GetConnection();
    }
}
