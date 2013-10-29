using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FolderLogic
    {
        FSMasterDBContext db = new FSMasterDBContext();
        public Folder CreateIfNotExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            else
            {
                var v = db.Folders.Where(r => r.Path == path).FirstOrDefault();
                
                if (v == null)
                {
                    v = db.Folders.Add(new Folder() { Path = path });
                    db.SaveChanges();
                }
                
                return v;
            }            
        }
    }
}
