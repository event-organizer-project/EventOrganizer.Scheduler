using MySqlConnector;
using System.Data.Common;

namespace EventOrganizer.Scheduler.DataAccess
{
    public class MySqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string connectionString;

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("connectionString");
        }

        public DbConnection CreateConnection()
        {
            var connection = new MySqlConnection(connectionString);

            return connection;
        }
    }
}
