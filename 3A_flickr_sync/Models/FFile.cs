using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public partial class FFile
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string HashCode { get; set; }
        //public Nullable<int> Status { get; set; }
    }
}
