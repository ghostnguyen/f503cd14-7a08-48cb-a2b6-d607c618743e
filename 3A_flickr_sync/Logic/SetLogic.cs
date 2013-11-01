using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;
using _3A_flickr_sync.FlickrNet;

namespace _3A_flickr_sync.Logic
{
    public class SetLogic : FSDBLogic
    {
        public Set AddOrUpdate(Photoset set)
        {
            var v = db.Sets.FirstOrDefault(r => r.UserID == Flickr.UserId && r.SetsID == set.PhotosetId);

            if (v == null)
            {
                db.Sets.Add(new Set() { UserID = Flickr.UserId, SetsID = set.PhotosetId, Tittle = set.Title });
            }
            else
            {
                v.Tittle = set.Title;
            }

            db.SaveChanges();

            return v;
        }

        public void AddOrUpdate(PhotosetCollection setL)
        {
            foreach (var item in setL)
            {
                AddOrUpdate(item);
            }
        }

        public string GetTittleFromPath(string path)
        {
            var root = Path.GetPathRoot(path);
            if (string.IsNullOrEmpty(root))
            {
                throw new Exception("");
            }
            else
            {
                return "";
            }
        }
    }
}
