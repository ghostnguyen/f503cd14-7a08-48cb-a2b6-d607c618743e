using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FUserLogic
    {
        FSMasterDBContext db = new FSMasterDBContext();
        public FUser CreateOrUpdate(string userID, string oAuthAccessToken, string oAuthAccessTokenSecret, string userName, string fullName)
        {
            FUser v = db.FUsers.FirstOrDefault();

            if (v == null)
            {

            }
            else
            {
                db.FUsers.Remove(v);
            }

            v = db.FUsers.Add(new FUser() { UserId = userID, OAuthAccessToken = oAuthAccessToken, OAuthAccessTokenSecret = oAuthAccessTokenSecret });

            db.SaveChanges();

            return v;
        }

        public FUser GetFirst()
        {
            FUser v = db.FUsers.FirstOrDefault();
            return v;
        }
    }
}
