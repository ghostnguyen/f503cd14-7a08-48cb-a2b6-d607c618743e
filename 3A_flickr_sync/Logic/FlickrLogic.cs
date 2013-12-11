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
        static public bool IsUpload { get; set; }
        static public CancellationTokenSource CancellationTokenSrc;
        static public CancellationToken CancellationToken;

        static public void ResetCancellationToken()
        {
            IsUpload = false;
            CancellationTokenSrc = new CancellationTokenSource();
            CancellationToken = CancellationTokenSrc.Token;
        }

        static public void StopUpload()
        {
            FlickrLogic.IsUpload = false;
            FlickrLogic.CancellationTokenSrc.Cancel();
        }

        static FlickrLogic()
        {
            ResetCancellationToken();

            MaxUpload = 3;

            var networkStatus = Observable.Interval(TimeSpan.FromSeconds(3)).Where(r => IsNetworkOk == false && IsUpload)
                .Subscribe(r => IsNetworkOk = Helper.CheckForInternetConnection());

            var interval = Observable.Interval(TimeSpan.FromSeconds(1));

            var v13 = uploadTaskList.ObservesChanged()
                .Select(r => (long)-1)
                .Merge(interval)
                .Where(r => IsUpload && uploadTaskList.Count < MaxUpload
                )
                .Subscribe(r =>
                {
                    if (r > 0 && uploadTaskList.Count != 0)
                    {
                        //Do not proceess event come from Interval  
                        //if uploadTaskList has already run.
                    }
                    else
                    {
                        var file = FFileLogic.DequeueForUpload();
                        if (file == null) { }
                        else
                        {
                            FlickrLogic logic = new FlickrLogic(file.Item1);

                            var task = logic.Upload(file.Item2.Id);
                            task.ContinueWith(r1 => 
                                {
                                    uploadTaskList.Remove(r1);
                                }
                                );

                            uploadTaskList.Add(task);
                        }
                    }
                }
                );
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
                                progress.ProgressChanged += ((a, b) => { FlickrLogic.UploadEventList.Enqueue(new Notice() { UploadProgress = b, FullPath = file.Path }); });

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
                                    file.UserID = Flickr.User.UserId;
                                    db.SaveChanges();

                                    UpdateSets(file.Id);
                                }
                            }
                        }
                        catch (WebException ex)
                        {
                            FlickrLogic.UploadEventList.Enqueue(new Notice() { Ex = ex, FullPath = file.Path });
                        }
                        catch (Exception ex)
                        {
                            FlickrLogic.UploadEventList.Enqueue(new Notice() { Ex = ex, FullPath = file.Path });
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

        async static public Task StartUpload()
        {
            ResetCancellationToken();

            if (FlickrLogic.IsUpload)
            {

            }
            else
            {
                FlickrLogic.IsUpload = true;

                var currentFolderId = 0;
                while (true)
                {
                    if (CancellationToken != null)
                        CancellationToken.ThrowIfCancellationRequested();

                    var folder = FFolderLogic.GetForCurrentUser(currentFolderId);
                    if (folder == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        FFileLogic fLogic = new FFileLogic(folder);
                        await fLogic.StartBuffer(CancellationToken);
                    }
                }
            }
        }
    }
}
