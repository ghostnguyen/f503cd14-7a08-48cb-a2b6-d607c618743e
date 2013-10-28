using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public partial class File
    {
        public string HashID { get; set; }
        public string Path { get; set; }
        public Nullable<System.DateTime> SyncDate { get; set; }
    }
}
