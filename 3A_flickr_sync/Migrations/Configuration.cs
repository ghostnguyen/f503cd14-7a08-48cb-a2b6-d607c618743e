//namespace _3A_flickr_sync.Migrations
namespace _3A_flickr_sync.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    public partial class FSDBContext : DbContext
    {
        internal sealed class Configuration : DbMigrationsConfiguration<_3A_flickr_sync.Models.FSDBContext>
        {
            public Configuration()
            {
                AutomaticMigrationsEnabled = true;
            }

            protected override void Seed(_3A_flickr_sync.Models.FSDBContext context)
            {
                //  This method will be called after migrating to the latest version.

                //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
                //  to avoid creating duplicate seed data. E.g.
                //
                //    context.People.AddOrUpdate(
                //      p => p.FullName,
                //      new Person { FullName = "Andrew Peters" },
                //      new Person { FullName = "Brice Lambson" },
                //      new Person { FullName = "Rowan Miller" }
                //    );
                //
            }
        }
    }
}
