using System.Data.Common;

namespace EventOrganizer.Scheduler.DataAccess
{
    public interface ISqlConnectionFactory
    {
        DbConnection CreateConnection();
    }
}
