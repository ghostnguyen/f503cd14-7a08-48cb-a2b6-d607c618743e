namespace _3A_flickr_sync.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class ConfigurationMaster : DbMigrationsConfiguration<_3A_flickr_sync.Models.FSMasterDBContext>
    {
        public ConfigurationMaster()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "_3A_flickr_sync.Models.FSMasterDBContext";
        }

        protected override void Seed(_3A_flickr_sync.Models.FSMasterDBContext context)
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
