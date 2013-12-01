using System;
using System.Collections.Generic;

namespace _3A_flickr_sync.Models
{
    public enum FFileStatus
    {
        New = 0,
        UploadNoSets = 10,
        UploadInSets = 11,
        HashCodeFound = 20,
    }

    public partial class FFile
    {
        public Nullable<FFileStatus> Status { get; set; }

        public string GetPathId(string folderPath)
        {
            return string.Format("{0}\\{1}", folderPath, Id);
        }
    }



}
