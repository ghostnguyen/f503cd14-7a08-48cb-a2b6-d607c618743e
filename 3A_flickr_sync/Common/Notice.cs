using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.Common
{
    public class Notice
    {
        public DateTime Datetime { get; private set; }
        public string Id { get; set; }
        public Exception Ex { get; set; }
        public UploadProgressChangedEventArgs UploadProgress { get; set; }

        public Notice()
        {
            Datetime = DateTime.Now;
        }
    }
}
