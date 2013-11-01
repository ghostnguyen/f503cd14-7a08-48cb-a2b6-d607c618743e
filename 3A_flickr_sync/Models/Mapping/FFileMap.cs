using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace _3A_flickr_sync.Models.Mapping
{
    public class FFileMap : EntityTypeConfiguration<FFile>
    {
        public FFileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("File");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.HashCode).HasColumnName("HashCode");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.HashCodeNoExif).HasColumnName("HashCodeNoExif");
            this.Property(t => t.PhotoID).HasColumnName("PhotoID");
            this.Property(t => t.SetsID).HasColumnName("SetsID");
            this.Property(t => t.UserID).HasColumnName("UserID");
        }
    }
}
