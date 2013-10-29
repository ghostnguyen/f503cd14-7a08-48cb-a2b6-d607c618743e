using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var v = db.FFolders.Where(r => r.Path == path).FirstOrDefault();
                
                if (v == null)  
                {
                    v = db.FFolders.Add(new FFolder() { Path = path });
                    db.SaveChanges();
                }
                
                return v;
            }            
        }
    }
}
