using _3A_flickr_sync.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3A_flickr_sync.Models
{
    public partial class FFolder
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        public string UserId { get; set; }
    }
}
