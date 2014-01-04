using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Models;
using _3A_flickr_sync.Common;

namespace _3A_flickr_sync.Logic
{
    public class FFolderLogic
    {
        //FSMasterDBContext db = new FSMasterDBContext();
        static FSMasterDBContext db = new FSMasterDBContext();
        public FFolder CreateIfNotExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            else
            {
                //Not exist
                //Not exist parent
                var v = db.FFolders.Where(r => (path.Contains(r.Path) || r.Path == path) && r.UserId == Flickr.User.UserId).FirstOrDefault();

                if (v == null)
                {
                    v = db.FFolders.Add(new FFolder() { Path = path, UserId = Flickr.User.UserId });
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Folder or parent folder existing.");
                }

                return v;
            }
        }

        public void Delete(int Id)
        {
            var e = db.FFolders.Single(r => r.Id == Id);
            db.FFolders.Remove(e);
            db.SaveChanges();
        }

        //public static FFolder GetForCurrentUser(int excludeID)
        //{
        //    return db.FFolders.FirstOrDefault(r => r.Id != excludeID && r.UserId == Flickr.User.UserId);
        //}

        public static FFolder GetForUpload()
        {
            var v = db.FFolders.FirstOrDefault(r => r.ProcessingStatus == null && r.UserId == Flickr.User.UserId);
            if (v == null)
            { }
            else
            {
                v.ProcessingStatus = ProcessingStatus.Processing;
                db.SaveChanges();
            }
            return v;
        }

        public static void Reset_ProcessingStatus()
        {
            var v = db.FFolders.Where(r => r.ProcessingStatus != null);

            foreach (var item in v)
            {
                item.ProcessingStatus = null;
            }
            db.SaveChanges();
        }
    }
}
