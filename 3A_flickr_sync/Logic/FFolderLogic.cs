using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FFolderLogic
    {
        FSMasterDBContext db = new FSMasterDBContext();
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


    }
}
