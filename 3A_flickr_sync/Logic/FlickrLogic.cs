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
using System.Collections.Concurrent;
using System.Threading;

namespace _3A_flickr_sync.Logic
{
    public class FlickrLogic : FSDBLogic
    {
        static private ObservableCollection<Task<FFile>> uploadTaskList = new ObservableCollection<Task<FFile>>();
        static public ConcurrentQueue<Notice> UploadEventList = new ConcurrentQueue<Notice>();
        static public bool IsNetworkOk { get; set; }
        static public int MaxUpload { get; set; }

        static FlickrLogic()
        {
            MaxUpload = 3;

            var networkStatus = Observable.Interval(TimeSpan.FromSeconds(3)).Where(r => IsNetworkOk == false)
                .Subscribe(r => IsNetworkOk = Helper.CheckForInternetConnection());

            var v13 = uploadTaskList.ObservesChanged()
                        .Where(r => ((ObservableCollection<FFile>)r.Sender).Count < MaxUpload)
                        .Subscribe(r =>
                        {
                            var file = FFileLogic.DequeueForUpload();
                            if (file == null) { }
                            else
                            {
                                FlickrLogic logic = new FlickrLogic(file.Item1);

                                var task = logic.Upload(file.Item2.Id);
                                task.ContinueWith(r1 => uploadTaskList.Remove(r1));

                                uploadTaskList.Add(task);
                            }
                        }
                        )
                        ;
        }

        public FlickrLogic(string path)
            : base(path)
        {
        }

        public async Task<FFile> Upload(int fFileID)
        {
            while (IsNetworkOk == false)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }

            var file = db.FFiles.FirstOrDefault(r => r.Id == fFileID);

            if (file != null && file.Status == FFileStatus.New)
            {
                if (file.Path.Contains(db.Path))
                {
                    Flickr flickr = new Flickr();

                    FileInfo fileInfo = new FileInfo(file.Path);
                    if (fileInfo.Exists)
                    {
                        try
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

                                var progress = new Progress<UploadProgressChangedEventArgs>();
                                progress.ProgressChanged += ((a, b) => { FlickrLogic.UploadEventList.Enqueue(new Notice() { Id = file.Id, DbPath = db.Path, UploadProgress = b }); });

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
                        catch (WebException ex)
                        {
                            FlickrLogic.UploadEventList.Enqueue(new Notice() { Id = file.Id, DbPath = db.Path, Ex = ex });
                        }
                        catch (Exception ex)
                        {
                            FlickrLogic.UploadEventList.Enqueue(new Notice() { Id = file.Id, DbPath = db.Path, Ex = ex });
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
                SetLogic l = new SetLogic();
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

        async static public Task StartUpload(CancellationToken cancellationToken)
        {
            var currentId = 0;
            while (true)
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                FSMasterDBContext masterDB = new FSMasterDBContext();
                var folder = masterDB.FFolders.FirstOrDefault(r => r.Id != currentId);
                if (folder == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                else
                {
                    FFileLogic fLogic = new FFileLogic(folder);
                    await fLogic.StartBuffer(cancellationToken);
                }
            }
        }
    }
}
