using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace _3A_flickr_sync.Models.Mapping
{
    public class SetMap : EntityTypeConfiguration<Set>
    {
        public SetMap()
        {
            // Primary Key
            this.HasKey(t => t.SetsID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Set");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Tittle).HasColumnName("Tittle");
        }
    }
}
