using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3A_flickr_sync.Models
{
    public partial class FUser
    {
        [Key]
        public string UserId { get; set; }
        public string OAuthAccessToken { get; set; }
        public string OAuthAccessTokenSecret { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string DownloadPath { get; set; }
    }
}
