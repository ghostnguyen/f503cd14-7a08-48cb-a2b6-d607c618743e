using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace _3A_flickr_sync.Models.Mapping
{
    public class FUserMap : EntityTypeConfiguration<FUser>
    {
        public FUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            // Table & Column Mappings
            this.ToTable("User");
            this.Property(t => t.UserId).HasColumnName("Path");
            this.Property(t => t.OAuthAccessToken).HasColumnName("OAuthAccessToken");
            this.Property(t => t.OAuthAccessTokenSecret).HasColumnName("OAuthAccessTokenSecret");
        }
    }
}
