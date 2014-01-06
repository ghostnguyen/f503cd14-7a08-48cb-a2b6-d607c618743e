using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3A_flickr_sync.Models
{
    public partial class Set
    {
        [Key]
        public string SetsID { get; set; }
        public string UserID { get; set; }
        public string Tittle { get; set; }
    }
}
