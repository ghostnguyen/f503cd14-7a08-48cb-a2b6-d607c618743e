using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public enum FFileStatus
    {
        New = 0,
    }

    public partial class FFile
    {
        public Nullable<FFileStatus> Status { get; set; }
    }
}
