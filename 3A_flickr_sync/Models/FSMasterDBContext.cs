using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace _3A_flickr_sync.Models
{
    public partial class FSMasterDBContext : DbContext
    {
        static FSMasterDBContext()
        {
            Database.SetInitializer<FSMasterDBContext>(null);
        }

        //public FSMasterDBContext()
        //    : base("Name=FSMasterDBContext")
        //{
        //}

        public DbSet<FFolder> FFolders { get; set; }
        public DbSet<FUser> FUsers { get; set; }
        public DbSet<Set> Sets { get; set; }
    }
}
