using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace _3A_flickr_sync.Models.Mapping
{
    public class FileMap : EntityTypeConfiguration<File>
    {
        public FileMap()
        {
            // Primary Key
            this.HasKey(t => t.HashID);

            // Properties
            this.Property(t => t.HashID)
                .IsRequired()
                .HasMaxLength(900);

            // Table & Column Mappings
            this.ToTable("File");
            this.Property(t => t.HashID).HasColumnName("HashID");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.SyncDate).HasColumnName("SyncDate");
        }
    }
}
