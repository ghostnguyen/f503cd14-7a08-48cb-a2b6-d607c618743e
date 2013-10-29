using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}
