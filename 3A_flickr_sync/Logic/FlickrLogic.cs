﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync.Logic
{
    public class FlickrLogic : FSDBLogic
    {
        public FFile Upload(int fFileID)
        {
            var file = db.FFiles.FirstOrDefault(r => r.Id == fFileID);

            if (file != null && file.Status == FFileStatus.New)
            {
                Flickr flickr = new Flickr();

                FileInfo fileInfo = new FileInfo(file.Path);
                if (fileInfo.Exists)
                {
                    var hashCode = Helper.HashFile(file.Path);
                    var hashCodeNoExif = Helper.HashPhotoNoExif(file.Path);

                    if (ExistFile(hashCode))
                    {
                        file.Status = FFileStatus.HashCodeFound;
                        db.SaveChanges();
                    }
                    else
                    {
                        var tags = string.Format("MD5:{0} MD5NoExif:{1}", hashCode, hashCodeNoExif);
                        var photoID = flickr.UploadPicture(file.Path, tags: tags);

                        if (string.IsNullOrEmpty(photoID))
                        { }
                        else
                        {
                            file.PhotoID = photoID;
                            file.HashCode = hashCode;
                            file.HashCodeNoExif = hashCodeNoExif;
                            file.Status = FFileStatus.UploadNoSets;
                            file.UserID = Flickr.UserId;
                            db.SaveChanges();
                        }
                    }
                }
            }

            return file;
        }

        //public FFile UpdateSets(int fFileID)
        //{
        //    var file = db.FFiles.FirstOrDefault(r => r.Id == fFileID);

        //    if (file != null && file.Status == FFileStatus.UploadNoSets)
        //    {
        //        Flickr flickr = new Flickr();
                
        //        FileInfo fileInfo = new FileInfo(file.Path);
        //        //fileInfo.DirectoryName.Remove(
                
        //        Path.GetPathRoot(fileInfo.FullName);
                
                

        //    }

        //    return file;
        //}

        public void DownloadSets()
        {
            Flickr flickr = new Flickr();
            var setList = flickr.PhotosetsGetList();

            SetLogic l = new SetLogic();
            l.AddOrUpdate(setList);
        }

        public bool ExistFile(string hashCode)
        {
            Flickr flickr = new Flickr();

            var op = new PhotoSearchOptions();
            op.Tags = string.Format("MD5:{0}", hashCode);

            var l = flickr.PhotosSearch(op);

            return l.Count() > 0;
        }
    }
}