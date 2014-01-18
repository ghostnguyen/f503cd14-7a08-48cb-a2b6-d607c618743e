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
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string userFilePath = Path.Combine(localAppData, "3A-Software");

            if (!Directory.Exists(userFilePath))
                Directory.CreateDirectory(userFilePath);

            SqlConnectionStringBuilder connStrBuilder = new SqlConnectionStringBuilder(AppSetting.FSMasterDBConnectionStr);

            FileInfo fileInfo = new FileInfo(connStrBuilder.AttachDBFilename);

            //connStrBuilder.AttachDBFilename = AppDomain.CurrentDomain.BaseDirectory + fileInfo.Name;
            connStrBuilder.AttachDBFilename = Path.Combine(userFilePath, fileInfo.Name);

            ////if it's not already there, 
            ////copy the file from the deployment location to the folder
            //string sourceFilePath = Path.Combine(
            //  System.Windows.Forms.Application.StartupPath, "MyFile.txt");
            //string destFilePath = Path.Combine(userFilePath, "MyFile.txt");
            //if (!File.Exists(destFilePath))
            //    File.Copy(sourceFilePath, destFilePath);

            return connStrBuilder.ConnectionString;
        }
    }
}
