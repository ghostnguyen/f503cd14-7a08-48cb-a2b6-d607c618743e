using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using _3A_flickr_sync.Models.Mapping;

namespace _3A_flickr_sync.Models
{
    public partial class FSDBContext : DbContext
    {
        static FSDBContext()
        {
            Database.SetInitializer<FSDBContext>(null);
        }

        public DbSet<FFile> FFiles { get; set; }
        public DbSet<Set> Sets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FFileMap());
            modelBuilder.Configurations.Add(new SetMap());
        }
    }
}
