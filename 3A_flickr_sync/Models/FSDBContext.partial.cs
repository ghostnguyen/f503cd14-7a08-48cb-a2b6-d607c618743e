using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models.Mapping;

namespace _3A_flickr_sync.Models
{
    public partial class FSDBContext : DbContext
    {
        /// <summary>
        /// Should delete this method from EF Tools
        /// </summary>
        private FSDBContext()
        {

        }

        public string Path { 
            get; 
            private set; }

        public FSDBContext(string path)
            : base(GetConnectionString(path))
        {
            Path = path;
            Database.CreateIfNotExists();
        }

        private static string GetConnectionString(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception(ErrMess.Err1);
            }
            else
            {
                if (Directory.Exists(path))
                {                    
                    var connStr = AppSetting.FSDBConnectionStr;
                    SqlConnectionStringBuilder connStrBuilder = new SqlConnectionStringBuilder(connStr);

                    FileInfo fileInfo = new FileInfo(connStrBuilder.AttachDBFilename);
                    connStrBuilder.AttachDBFilename = path + @"\" + fileInfo.Name;

                    return connStrBuilder.ConnectionString;                    
                }
                else
                {
                    throw new Exception(ErrMess.Err1);
                }
            }
            //return @"Data Source=(localdb)\v11.0;AttachDbFilename=D:\ghostnguyen\Pictures\SGI Photo - Can Gio\3A_Flickr_Sync_DB.mdf;Integrated Security=True;MultipleActiveResultSets=True";
        }
    }
}
