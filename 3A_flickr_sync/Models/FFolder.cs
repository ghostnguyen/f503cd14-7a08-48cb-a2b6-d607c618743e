using _3A_flickr_sync.Common;
using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public partial class FFolder
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string UserId { get; set; }
    }
}
