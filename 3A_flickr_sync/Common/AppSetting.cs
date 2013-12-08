using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using _3A_flickr_sync.FlickrNet;

namespace _3A_flickr_sync.Common
{
    public class AppSetting
    {
        public static string[] Extension
        {
            get
            {                
                var s = ConfigurationManager.AppSettings[MethodBase.GetCurrentMethod().Name.Substring(4)];
                return s.Split(';');
            }
        }

        public static string FSMasterDBConnectionStr = @"Data Source=(localdb)\v11.0;AttachDbFilename=3A_Flickr_Sync_Master_DB.mdf;Integrated Security=True;MultipleActiveResultSets=True";
        public static string FSDBConnectionStr = @"Data Source=(localdb)\v11.0;AttachDbFilename=3A_Flickr_Sync_DB.mdf;Integrated Security=True;MultipleActiveResultSets=True";

        
    }
}
