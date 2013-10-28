using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace _3A_flickr_sync.Models.Mapping
{
    public class FolderMap : EntityTypeConfiguration<Folder>
    {
        public FolderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Folder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Path).HasColumnName("Path");
        }
    }
}
