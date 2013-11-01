using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public partial class FUser
    {
        public string UserId { get; set; }
        public string OAuthAccessToken { get; set; }
        public string OAuthAccessTokenSecret { get; set; }
    }
}
