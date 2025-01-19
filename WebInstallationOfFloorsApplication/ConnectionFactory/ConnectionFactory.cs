using Npgsql;

namespace WebInstallationOfFloorsApplication;

public class ConnectionFactory: IConnectionFactory {
    private readonly string _connection;

    public ConnectionFactory(string connection) {
        _connection = connection;
    }

    public NpgsqlConnection GetConnection() {
        return new NpgsqlConnection(_connection);
    }

}