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
using System.Reactive.Subjects;

namespace _3A_flickr_sync.Logic
{
    public class FlickrLogic : FSDBLogic
    {
        static public ObservableCollection<Notice> UploadEventList = new ObservableCollection<Notice>();


        static public bool IsNetworkOk { get; set; }
        static public int MaxUpload { get; set; }
        static public int TotalUpload { get; set; }

        static public Subject<bool> IsUploadNotify;
        static bool _IsUpload;
        static public bool IsUpload
        {
            get
            {
                return _IsUpload;
            }
            set
            {
                if (_IsUpload == value)
                { }
                else
                {
                    _IsUpload = value;
                    if (IsUploadNotify == null)
                    {
                    }
                    else
                    {
                        IsUploadNotify.OnNext(value);
                    }
                }
            }
        }

        static public CancellationTokenSource CancellationTokenSrc;
        static public CancellationToken CancellationToken;
        static public string CurrentFolderPath { get; set; }

        FFileLogic fFileLogic;
        Flickr flickr;

        static public void ResetCancellationToken()
        {
            //IsUpload = false;
            TotalUpload = 0;
            CancellationTokenSrc = new CancellationTokenSource();
            CancellationToken = CancellationTokenSrc.Token;
        }

        //static public void StopUpload()
        //{
        //    FlickrLogic.IsUpload = false;
        //    FlickrLogic.CancellationTokenSrc.Cancel();
        //}

        static public void Log(string path, NoticeType type, string note)
        {
            FlickrLogic.UploadEventList.Add(new Notice() { Type = type, Note = note, FullPath = path });
        }

        static FlickrLogic()
        {
            ResetCancellationToken();

            MaxUpload = System.Environment.ProcessorCount;
            //MaxUpload = 1;

            var networkStatus = Observable.Interval(TimeSpan.FromSeconds(3)).Where(r => IsNetworkOk == false && IsUpload)
                .Subscribe(r =>
                    {
                        IsNetworkOk = Helper.CheckForInternetConnection();
                    }
                    );

            var interval = Observable.Interval(TimeSpan.FromSeconds(0.5));

            var v13 =
                interval
                .Where(r => IsUpload && TotalUpload < MaxUpload
                )
                .Subscribe(r =>
                {
                    Task.Run(async () =>
                    {
                        TotalUpload++;

                        if (string.IsNullOrEmpty(CurrentFolderPath))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            FFileLogic fFileLogic = new FFileLogic(CurrentFolderPath);
                            FlickrLogic logic = new FlickrLogic(CurrentFolderPath);
                            var file = fFileLogic.TakeBuffer();
                            if (file == null) { }
                            else if (file.Status == FFileStatus.New)
                            {
                                FlickrLogic.Log(CurrentFolderPath, NoticeType.Upload, "Waiting Upload");
                                await logic.Upload(file.Id);
                            }
                            else if (file.Status == FFileStatus.Existing)
                            {
                                FlickrLogic.Log(CurrentFolderPath, NoticeType.Upload, "Waiting HashCodeFound");
                                logic.Processing_HashCodeFound(file.Id);
                            }

                            //var file = FFileLogic.DequeueForUpload();
                            //if (file == null) { }
                            //else if (file.Item2.Status == FFileStatus.New)
                            //{
                            //    FlickrLogic.Log(file.Item2.Path, NoticeType.Upload, "Waiting Upload");
                            //    FlickrLogic logic = new FlickrLogic(file.Item1);
                            //    await logic.Upload(file.Item2.Id);
                            //}
                            //else if (file.Item2.Status == FFileStatus.Existing)
                            //{
                            //    FlickrLogic.Log(file.Item2.Path, NoticeType.Upload, "Waiting HashCodeFound");
                            //    FlickrLogic logic = new FlickrLogic(file.Item1);
                            //    logic.Processing_HashCodeFound(file.Item2.Id);
                            //}
                        }

                        TotalUpload--;
                    });
                });


            IsUploadNotify = new Subject<bool>();
            IsUploadNotify.Subscribe(r =>
            {
                if (r)
                {
                    StartUpload();
                }
                else
                {
                    FlickrLogic.CancellationTokenSrc.Cancel();
                }
            });

        }

        public FlickrLogic(string path)
            : base(path)
        {
            fFileLogic = new FFileLogic(path);
            flickr = new Flickr();
        }

        public async Task<FFile> Upload(int fFileID)
        {
            while (IsNetworkOk == false)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }

            var file = fFileLogic.GetForSure(fFileID);

            if (file == null)
            { }
            else
            {
                var isExisted = fFileLogic.CheckExistHashCode(ExistFile, fFileID);

                if (isExisted)
                {
                    file = fFileLogic.DoUpdate(fFileID, file1 =>
                    {
                        if (file1.Status == FFileStatus.New)
                        {
                            file1.Status = FFileStatus.Existing;
                            file1.ProcessingStatus = null;
                        }
                    });
                }
                else
                {
                    var progress = new Progress<UploadProgressChangedEventArgs>();

                    progress.ToObservable()
                        .DistinctUntilChanged(r => (int)r.EventArgs.UploadPercentage() / 5)
                        .Subscribe(r =>
                        {
                            FlickrLogic.UploadEventList.Add(new Notice()
                            {
                                Type = NoticeType.Upload,
                                JobDone = r.EventArgs.BytesSent,
                                JobTotal = r.EventArgs.TotalBytesToSend,
                                Percentage = r.EventArgs.UploadPercentage(),
                                FullPath = file.Path,
                                Note = "Uploading",
                            });
                        })
                    ;

                    var task = flickr.UploadPicture(file.Path, tags: file.GetHashCodeTag(), progress: progress);

                    var photoID = await task;

                    if (string.IsNullOrEmpty(photoID))
                    { }
                    else
                    {

                        file = fFileLogic.DoUpdate(fFileID, file1 =>
                        {
                            file1.PhotoID = photoID;
                            file1.Status = FFileStatus.Uploaded_NoSet;
                            file1.UserID = Flickr.User.UserId;
                        });

                        UpdateSets(file.Id);

                        FlickrLogic.Log(file.Path, NoticeType.UploadDone, "Uploaded");
                    }
                }
            }

            return file;
        }

        public FFile UpdateSets(int fileID)
        {
            var file = fFileLogic.Get(fileID);

            if (file != null
                && (file.Status == FFileStatus.Uploaded_NoSet 
                        || file.Status == FFileStatus.Uploaded_InSet)
                )
            {
                FlickrLogic.Log(file.Path, NoticeType.Upload, "Add to photo sets");

                SetLogic l = new SetLogic();
                string setID = l.AddPhoto(file);

                if (string.IsNullOrEmpty(setID))
                { }
                else
                {
                    file = fFileLogic.DoUpdate(fileID, file1 =>
                    {
                        file1.SetsID = setID;
                        file1.Status = FFileStatus.Uploaded_SyncSet;
                    });
                }
            }

            return file;
        }

        public FFile Processing_HashCodeFound(int fileID)
        {
            var file = fFileLogic.UpdateHashCode(fileID);

            if (file != null && file.Status == FFileStatus.Existing)
            {
                FlickrLogic.Log(file.Path, NoticeType.Upload, "Checking HashCodeFound");

                file = fFileLogic.UpdateFlickrIds(fileID, GetPhoto_ByHashCode);
                fFileLogic.UpdateStatus(fileID);

                if (string.IsNullOrEmpty(file.PhotoID))
                { }
                else
                {
                    var context = flickr.PhotosGetAllContexts(file.PhotoID);
                    if (context == null)
                    { }
                    else
                    {
                        if (context.Sets.Count > 0)
                        {
                            file = fFileLogic.DoUpdate(fileID, file1 =>
                            {
                                file1.Status = FFileStatus.Uploaded_InSet;
                            });
                        }
                        else
                        {
                            file = fFileLogic.DoUpdate(fileID, file1 =>
                            {
                                file1.Status = FFileStatus.Uploaded_NoSet;
                            });
                        }

                        UpdateSets(file.Id);
                    }
                }

                FlickrLogic.Log(file.Path, NoticeType.UploadDone, "Done " + file.Status.ToString());
            }

            return file;
        }

        public bool ExistFile(string hashCode)
        {
            var l = GetPhoto_ByHashCode(hashCode);

            return l.Count() > 0;
        }

        public PhotoCollection GetPhoto_ByHashCode(string hashCode)
        {
            Flickr flickr = new Flickr();

            var op = new PhotoSearchOptions();
            op.Tags = string.Format("MD5:{0}", hashCode);

            var l = flickr.PhotosSearch(op);

            return l;
        }

        async static Task StartUpload()
        {
            ResetCancellationToken();

            //wait for all upload complete if remaining
            await WaitForComplete();

            FFolderLogic.Reset_ProcessingStatus();

            while (FlickrLogic.IsUpload)
            {
                var folder = FFolderLogic.GetForUpload();
                if (folder == null)
                {
                    await WaitForComplete();

                    FlickrLogic.IsUpload = false;
                }
                else
                {
                    CurrentFolderPath = folder.Path;
                    FFileLogic fLogic = new FFileLogic(folder);
                    await fLogic.StartBuffer(CancellationToken);
                }
            }
        }

        private static async Task WaitForComplete()
        {
            while (TotalUpload > 0 || FFileLogic.QueueCount() > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
