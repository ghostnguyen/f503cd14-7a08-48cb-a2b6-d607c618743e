using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Models;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Reactive.Concurrency;

namespace _3A_flickr_sync.Logic
{
    public class FlickrLogic : FSDBLogic
    {
        public FlickrLogic(string path)
            : base(path)
        { }

        public int MaxUpload { get; set; }
        public TimeSpan Wait { get; set; }
        public int MinBuffer { get; set; }

        public async Task Upload(IProgress<Tuple<int, UploadProgressChangedEventArgs>> progress)
        {
            //SetLogic setL = new SetLogic(db.Path);
            //setL.DownloadPhotsets();

            FFileLogic fileL = new FFileLogic(db.Path);

            var fList = new ObservableCollection<FFile>();
            var uploadTaskList = new ObservableCollection<Task<FFile>>();

            MinBuffer = 5;
            MaxUpload = 5;
            Wait = TimeSpan.FromSeconds(5);

            var v12 = fList.ObservesChanged()
                .Select(r => new { r.EventArgs, List = (ObservableCollection<FFile>)r.Sender })
                .Where(r => r.EventArgs.Action == NotifyCollectionChangedAction.Remove
                        && r.List.Count < MinBuffer)
                .Throttle(Wait);

            v12.Subscribe(r => fileL.TakeNew(3).ForEach(r1 => r.List.Add(r1)));

            var v13 = uploadTaskList.ObservesChanged()
                .Where(r => r.EventArgs.Action == NotifyCollectionChangedAction.Remove
                && ((ObservableCollection<FFile>)r.Sender).Count < MaxUpload)
                .Throttle(TimeSpan.FromSeconds(1));

            //NewThreadScheduler.Default.Schedule
            
            //db.FFiles.TakeWhile
            var fL = db.FFiles.Where(r => r.Status == FFileStatus.New && r.Path.Contains(db.Path))

                .ToListAsync();




            int maxUploadTask = 3;
            List<Task<FFile>> taskL = new List<Task<FFile>>();
            int c = 0;


            //foreach (var item in fL)
            //{
            //    if (c < maxUploadTask)
            //    {
            //        var subProgress = new Progress<UploadProgressChangedEventArgs>(r => { progress.Report(new Tuple<int, UploadProgressChangedEventArgs>(item.Id, r)); });

            //        var v1 = Upload(item.Id, subProgress);
            //        taskL.Add(v1);
            //        c++;
            //    }
            //    else if (c == maxUploadTask)
            //    {
            //        await Task.WhenAny(taskL);
            //        c--;
            //        taskL = new List<Task<FFile>>();
            //    }
            //}
        }

        //public async Task Upload(IProgress<Tuple<int, UploadProgressChangedEventArgs>> progress)
        //{
        //    SetLogic setL = new SetLogic(db.Path);
        //    setL.DownloadPhotsets();

        //    //db.FFiles.TakeWhile
        //    var fL = db.FFiles.Where(r => r.Status == FFileStatus.New && r.Path.Contains(db.Path))
        //        .ToList();

        //    foreach (var item in fL)
        //    {
        //        if (c < maxUploadTask)
        //        {
        //            var subProgress = new Progress<UploadProgressChangedEventArgs>(r => { progress.Report(new Tuple<int, UploadProgressChangedEventArgs>(item.Id, r)); });

        //            var v1 = Upload(item.Id, subProgress);
        //            taskL.Add(v1);
        //            c++;
        //        }
        //        else if (c == maxUploadTask)
        //        {
        //            await Task.WhenAny(taskL);
        //            c--;
        //            taskL = new List<Task<FFile>>();
        //        }
        //    }
        //}

        public async Task<FFile> Upload(int fFileID, IProgress<UploadProgressChangedEventArgs> progress)
        {
            var file = db.FFiles.FirstOrDefault(r => r.Id == fFileID);

            if (file != null && file.Status == FFileStatus.New)
            {
                if (file.Path.Contains(db.Path))
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

                            var task = flickr.UploadPicture(file.Path, tags: tags, progress: progress);

                            var photoID = await task;

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

                                UpdateSets(file.Id);
                            }
                        }
                    }
                }
            }

            return file;
        }

        public FFile UpdateSets(int fileID)
        {
            var file = db.FFiles.FirstOrDefault(r => r.Id == fileID);
            if (file != null && file.Status == FFileStatus.UploadNoSets)
            {
                SetLogic l = new SetLogic(db.Path);
                string setID = l.AddPhoto(file);

                file.SetsID = setID;
                file.Status = FFileStatus.UploadInSets;
                db.SaveChanges();
            }

            return file;
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
