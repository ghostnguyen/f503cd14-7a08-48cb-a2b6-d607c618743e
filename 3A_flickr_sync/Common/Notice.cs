﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.Common
{
    public enum NoticeType
    {
        Upload,
        UploadDone,
        AddFile,
        AddFileDone,
        UploadException,
        Exception,
    }

    public class Notice
    {
        public DateTime Datetime { get; private set; }

        public NoticeType Type { get; set; }
        public string FullPath { get; set; }
        public string Note { get; set; }
        public long JobDone { get; set; }
        public long JobTotal { get; set; }
        public float Percentage { get; set; }

        //public UploadProgressChangedEventArgs UploadProgress { get; set; }

        public Notice()
        {
            Datetime = DateTime.Now;
        }

        public string GetNote()
        {
            string s = "";
            if (Type == NoticeType.UploadDone)
            {
                s = string.Format("{0} Uploaded.", FullPath);
            }
            if (Type == NoticeType.AddFileDone)
            {
                s = string.Format("{0}: {1} files added.", FullPath, JobDone.ToString());
            }
            if (Type == NoticeType.UploadException || Type == NoticeType.Exception)
            {
                s = string.Format("{0}: {1}", FullPath, Note);
            }
            s = Datetime.ToString() + " " + s;
            return s;
        }
    }
}
