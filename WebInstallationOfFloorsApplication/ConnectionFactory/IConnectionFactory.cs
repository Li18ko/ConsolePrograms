using Npgsql;

namespace WebInstallationOfFloorsApplication;

public interface IConnectionFactory {
    NpgsqlConnection GetConnection();
}