using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FSDBLogic
    {
        internal FSDBContext db = null;

        public string SyncPath { get { return db == null ? "" : db.Fullpath; } }

        public FSDBLogic(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("");
            }
            db = new FSDBContext(path);
        }
    }
}
