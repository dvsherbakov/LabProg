using System.Data.Entity;

namespace LabControl.DataModels
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
    }
}
