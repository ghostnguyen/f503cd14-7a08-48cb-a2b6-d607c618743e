using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using _3A_flickr_sync.Models.Mapping;

namespace _3A_flickr_sync.Models
{
    public partial class FSMasterDBContext : DbContext
    {
        static FSMasterDBContext()
        {
            Database.SetInitializer<FSMasterDBContext>(null);
        }

        public FSMasterDBContext()
            : base("Name=FSMasterDBContext")
        {
        }

        public DbSet<FFolder> Folders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FFolderMap());
        }
    }
}
