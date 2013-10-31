using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Test the logged in state of the current Filckr object.
        /// </summary>
        /// <returns>The <see cref="FoundUser"/> object containing the username and userid of the current user.</returns>
        public FoundUser TestLogin()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.test.login");

            return GetResponse<FoundUser>(parameters);
        }
    }
}
