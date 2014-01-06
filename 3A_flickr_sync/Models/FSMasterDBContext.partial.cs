using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.Configuration;
using System.Data.SqlClient;
using _3A_flickr_sync.Common;
using System.IO;
using System;

namespace _3A_flickr_sync.Models
{
    public partial class FSMasterDBContext : DbContext
    {
        public FSMasterDBContext()
            : base(GetConnectionString())
        {
        }

        private static string GetConnectionString()
        {
            SqlConnectionStringBuilder connStrBuilder = new SqlConnectionStringBuilder(AppSetting.FSMasterDBConnectionStr);

            FileInfo fileInfo = new FileInfo(connStrBuilder.AttachDBFilename);

            connStrBuilder.AttachDBFilename = AppDomain.CurrentDomain.BaseDirectory + fileInfo.Name;

            return connStrBuilder.ConnectionString;
        }
    }
}
