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
        public SetLogic(string path)
            : base(path)
        { }

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

        public void DownloadPhotsets()
        {
            Flickr f = new Flickr();
            var l = f.PhotosetsGetList();
            AddOrUpdate(l);
        }

        public string GetTittleFromPath(string path)
        {
            var title = "";
            var root = Path.GetPathRoot(path);
            var dirName = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(root) ||
                string.IsNullOrEmpty(dirName))
            {
                throw new Exception("");
            }
            else
            {
                if (root == dirName)
                {
                    title = "_root";
                }
                else
                {
                    title = dirName.Remove(0, root.Count());
                }
            }

            return title;
        }

        /// <summary>
        /// return SetID
        /// </summary>
        /// <param name="file"></param>
        /// <returns>SetID</returns>
        public string AddPhoto(FFile file)
        {
            var tittle = GetTittleFromPath(file.Path);
            var set = db.Sets.FirstOrDefault(r => r.UserID == Flickr.UserId && r.Tittle.ToLower() == tittle.ToLower());
            Flickr f = new Flickr();

            if (set == null)
            {
                var fSet = f.PhotosetsCreate(tittle, file.PhotoID);
                if (fSet != null)
                {
                    set = db.Sets.Add(new Set() { SetsID = fSet.PhotosetId, UserID = Flickr.UserId, Tittle = tittle });
                    db.SaveChanges();
                }
            }
            else
            {
                f.PhotosetsAddPhoto(set.SetsID, file.PhotoID); 
            }

            
            return set.SetsID;
        }
    }
}
