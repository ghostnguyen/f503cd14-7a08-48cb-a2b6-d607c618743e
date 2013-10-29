using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FFileLogic
    {
        
        FSDBContext db = new FSDBContext();
        public void FileLogic(FFolder folder)
        {
            if (folder == null || string.IsNullOrEmpty(folder.Path))
            {
                throw new Exception(ErrMess.Err1);
            }
            else
            {
                 
            }
        }
    }
}
