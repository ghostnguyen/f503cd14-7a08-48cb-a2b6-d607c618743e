using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public enum FFileStatus
    {
        New = 0,
        Uploaded_NoSet = 10,
        Uploaded_SyncSet = 11,
        Uploaded_InSet = 13,
        HashCodeFound = 20,
    }

    public partial class FFile
    {
        public Nullable<FFileStatus> Status { get; set; }

        //public string GetPathId(string folderPath)
        //{
        //    return string.Format("{0}\\{1}", folderPath, Id);
        //}
    }



}
