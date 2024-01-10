using Microsoft.EntityFrameworkCore;

namespace ProtoProductLib.BaseStruct
{
    public class ProductContext:DbContext
    {
     //   private User ContextOwner;
      // public DbSet<Work> Works { get; set; }
       // public DbSet<Post> Posts { get; set; }
//        public DbSet<User> Users { get; set; }
 //       public DbSet<Role> Roles { get; set; }
      // public DbSet<PostCreationKey> PostKeys { get; set; }
//        public DbSet<WorkLog> WorkLogs { get; set; }
/*
        private User SystemUser()
        {
            var exist = this.Users.FirstOrDefault(x => x.AccName == "system");
            if (exist == null)
            {
                exist = new User();
                exist.AccName = "system";
                exist.Name = "Система";
                exist.Mail = "product_post@ksk.ru";
                this.Users.Add(exist);
                this.SaveChanges();
            }

            return exist;
        }*/
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
            
        }

        public ProductContext() 
        {
       //     string accOwnerName = "";
      //      if (String.IsNullOrEmpty(accOwnerName))
            {
               // this.ContextOwner = SystemUser();
            }
      //      else
            {
              //  this.ContextOwner = this.Users.FirstOrDefault(x => x.AccName == accOwnerName);    
            }
            //this.SavingChanges+= OnSavingChanges;
        }

     //   private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
  //      {
            //WorkLog();
  //      }
/*
        private void WorkLog()
        {
            var changedWorks = this.ChangeTracker.Entries()
                .Where(x => Equals(x.Entity, Works))
                .Where(x => x.State is EntityState.Modified or EntityState.Added);
            List<WorkLog> logs = new List<WorkLog>();
            foreach (var change in changedWorks)
            {
                if (change.Entity is Work w)
                {
                    var statusChange = change.Properties.FirstOrDefault(x => x.Metadata.Name == "Status");
                    if (change.State == EntityState.Modified && statusChange != null)
                    {
                        
                        var log = new WorkLog();
                        log.Stamp = DateTime.Now;
                        log.Work = w;
                        log.NewStatus = (WorkStatus)statusChange.CurrentValue;
                        log.PrevStatus = (WorkStatus) statusChange.OriginalValue;
                     //   log.EditedBy = ContextOwner;
                        logs.Add(log);
                    }
                }
            }
            this.WorkLogs.AddRange(logs);
            

        } */


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            if (!optionsBuilder.IsConfigured)
            {
              //  string connectionString = Constants.Database.ConnectionString;
             //   Console.WriteLine(connectionString);
                optionsBuilder.UseSqlServer("Server=kdb1.kck2.ksk.ru;Database=protoproduct;User=sa;Password=-c2h5oh-");
              
            }
        }
    }
}