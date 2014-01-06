using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace _3A_flickr_sync.Models
{
    public partial class FSDBContext : DbContext
    {
        static FSDBContext()
        {
            Database.SetInitializer<FSDBContext>(null);
        }

        public DbSet<FFile> FFiles { get; set; }
    }
}
