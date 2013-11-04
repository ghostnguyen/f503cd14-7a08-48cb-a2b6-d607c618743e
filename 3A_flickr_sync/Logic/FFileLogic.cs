using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FFileLogic : FSDBLogic
    {
        public FFileLogic(FFolder folder)
            : base(folder.Path)
        {
        }

        public FFile Add(FileInfo file)
        {
            FFile v = null;
            if (file.Exists)
            {
                v = db.FFiles.Where(r => r.Path == file.FullName).FirstOrDefault();

                if (v == null)
                {
                    db.FFiles.Add(new FFile() { Path = file.FullName, Status = FFileStatus.New });
                    db.SaveChanges();
                }
            }
            return v;
        }

        public int Add(DirectoryInfo folder)
        {
            int c = 0;
            if (folder.Exists)
            {
                var ext = AppSetting.Extension;
                var f1 = Directory.EnumerateFiles(folder.FullName, "*.*", SearchOption.AllDirectories);
                
                foreach (var item in f1)
                {
                    var f = new FileInfo(item);
                    if (ext.Contains(f.Extension.ToLower()))
                    {                        
                        Add(f);
                    }
                }
                c = f1.Count();
            }
            return c;
        }
    }
}
