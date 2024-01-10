using Microsoft.EntityFrameworkCore;
using ProductLib.Person;
using ProductLib.UserRole;
using ProductLib.Work;

namespace ProductLib.BaseDataLayer
{
    public class BaseContext:DbContext
    {
        public BaseContext()
        {
            
        }
        
        public DbSet<WorkRecord> Works { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<OperatorRole> OperatorRoles { get; set; }
        public DbSet<PostMasterRole> PostMasterRoles { get; set; }
        public DbSet<User> Persons { get; set; }


        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Constants.Database.ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
              
            }
        }
    }
}