﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;
using _3A_flickr_sync.FlickrNet;

namespace _3A_flickr_sync.Logic
{
    public class SetLogic
    {
        static readonly object lockForCreateFlickrSets = new object();

        FSMasterDBContext db = new FSMasterDBContext();

        public Set AddOrUpdate(Photoset set)
        {
            var v = db.Sets.FirstOrDefault(r => r.UserID == Flickr.User.UserId && r.SetsID == set.PhotosetId);

            if (v == null)
            {
                db.Sets.Add(new Set() { UserID = Flickr.User.UserId, SetsID = set.PhotosetId, Tittle = set.Title });
            }
            else
            {
                if (v.Tittle != set.Title)
                    v.Tittle = set.Title;
            }

            db.SaveChanges();

            return v;
        }

        public void AddOrUpdate(PhotosetCollection setL)
        {
            foreach (var item in setL)
            {
                AddOrUpdate(item);
            }
        }

        public void RemoveNonExisting(PhotosetCollection setL)
        {
            var d = db.Sets.ToList().Where(r => (setL.Select(r1 => r1.PhotosetId).Contains(r.SetsID) == false)
                && r.UserID == Flickr.User.UserId);
            db.Sets.RemoveRange(d);
            db.SaveChanges();
        }

        public void DownloadPhotsets()
        {
            Flickr f = new Flickr();
            var l = f.PhotosetsGetList();

            RemoveNonExisting(l);
            AddOrUpdate(l);
        }

        public string GetTittleFromPath(string path)
        {
            var title = "";
            var root = Path.GetPathRoot(path);
            var dirName = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(root) ||
                string.IsNullOrEmpty(dirName))
            {
                throw new Exception("");
            }
            else
            {
                if (root == dirName)
                {
                    title = "_root";
                }
                else
                {
                    title = dirName.Remove(0, root.Count());
                }
            }

            return title;
        }

        public Set GetOrCreateThenAssign(string tittle, string photoID)
        {
            Set set;
            lock (lockForCreateFlickrSets)
            {
                Flickr f = new Flickr();
                set = db.Sets.FirstOrDefault(r => r.UserID == Flickr.User.UserId && r.Tittle.ToLower() == tittle.ToLower());
                if (set == null)
                {
                    var fSet = f.PhotosetsCreate(tittle, photoID);
                    if (fSet == null)
                    {

                    }
                    else
                    {
                        set = db.Sets.Add(new Set() { SetsID = fSet.PhotosetId, UserID = Flickr.User.UserId, Tittle = tittle });
                        db.SaveChanges();
                    }
                }
                else
                {
                    f.PhotosetsAddPhoto(set.SetsID, photoID);
                }
            }
            return set;
        }

        /// <summary>
        /// return SetID
        /// </summary>
        /// <param name="file"></param>
        /// <returns>SetID</returns>
        public string AddPhoto(FFile file)
        {
            var tittle = GetTittleFromPath(file.Path);
            var set = db.Sets.FirstOrDefault(r => r.UserID == Flickr.User.UserId && r.Tittle.ToLower() == tittle.ToLower());

            if (set == null)
            {
                set = GetOrCreateThenAssign(tittle, file.PhotoID);
            }
            else
            {
                Flickr f = new Flickr();
                f.PhotosetsAddPhoto(set.SetsID, file.PhotoID);
            }

            return set.SetsID;
        }

        public Set UpdateIsDownload(string setID, bool isDownload)
        {
            var v = db.Sets.FirstOrDefault(r => r.UserID == Flickr.User.UserId && r.SetsID == setID);

            if (v == null)
            {
                //db.Sets.Add(new Set() { UserID = Flickr.User.UserId, SetsID = set.PhotosetId, Tittle = set.Title });
            }
            else
            {
                v.IsDownload = isDownload;
                db.SaveChanges();
            }

            return v;
        }

        public void Reset_DownloadProcessingStatus()
        {
            var v = db.Sets.Where(r => r.ProcessingStatus != null);

            foreach (var item in v)
            {
                item.ProcessingStatus = null;
            }
            db.SaveChanges();
        }

        public Set GetForDownload()
        {
            var v = db.Sets.FirstOrDefault(r => r.ProcessingStatus == null && r.UserID == Flickr.User.UserId);
            if (v == null)
            { }
            else
            {
                v.ProcessingStatus = ProcessingStatus.Processing;
                db.SaveChanges();
            }
            return v;
        }

        public string GetDownloadFolderPath(string setId)
        {
            string path = "";
            var v = db.Sets.FirstOrDefault(r => r.SetsID == setId);
            if (v == null)
            { }
            else
            {
                if (string.IsNullOrEmpty(v.Path))
                {
                    FUserLogic uL = new FUserLogic();
                    var u = uL.GetFirst();
                    if (u == null) { }
                    else
                    {
                        if (string.IsNullOrEmpty(u.DownloadPath))
                        { }
                        else
                        {
                            if (Directory.Exists(u.DownloadPath))
                            {
                                path = Path.Combine(u.DownloadPath, v.Tittle);
                                try
                                {
                                    Directory.CreateDirectory(path);

                                    v.Path = path;
                                    db.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    path = "";
                                }
                            }
                        }
                    }
                }
                else
                {
                    path = v.Path;
                }
                
            }
            return path;
        }
    }
}
