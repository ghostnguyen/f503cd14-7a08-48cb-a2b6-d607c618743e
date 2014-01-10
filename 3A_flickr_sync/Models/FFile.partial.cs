using _3A_flickr_sync.Common;
using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public enum FFileStatus
    {
        New = 0,
        Existing = 20,
        Uploaded_SyncSet = 11,
        Uploaded_NoSet = 10,
        Uploaded_InSet = 13,
    }

    public partial class FFile
    {
        public Nullable<FFileStatus> Status { get; set; }
        public Nullable<ProcessingStatus> ProcessingStatus { get; set; }


        //public string GetPathId(string folderPath)
        //{
        //    return string.Format("{0}\\{1}", folderPath, Id);
        //}

        public string GetHashCodeTag()
        {
            return string.Format("MD5:{0} MD5NoExif:{1}", HashCode, HashCodeNoExif);
        }
    }





}
