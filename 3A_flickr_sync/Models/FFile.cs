using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3A_flickr_sync.Models
{
    public partial class FFile
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        public string HashCode { get; set; }
        public string HashCodeNoExif { get; set; }
        public string PhotoID { get; set; }
        public string SetsID { get; set; }
        public string UserID { get; set; }
    }
}
