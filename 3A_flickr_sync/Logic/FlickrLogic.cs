﻿using System;
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
        static public ObservableCollection<Notice> UploadEventList = new ObservableCollection<Notice>();

        static public bool IsNetworkOk { get; set; }
        static public int MaxUpload { get; set; }
        static public int TotalUpload { get; set; }
        static public bool IsUpload { get; set; }
        static public CancellationTokenSource CancellationTokenSrc;
        static public CancellationToken CancellationToken;

        FFileLogic fFileLogic;
        Flickr flickr;

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

        static public void Log(string path, NoticeType type, string note)
        {
            FlickrLogic.UploadEventList.Add(new Notice() { Type = type, Note = note, FullPath = path });
        }

        static FlickrLogic()
        {
            ResetCancellationToken();

            MaxUpload = System.Environment.ProcessorCount + 4;

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
                        var file = FFileLogic.DequeueForUpload();
                        if (file == null) { }
                        else
                        {
                            FlickrLogic.Log(file.Item2.Path, NoticeType.Upload, "Waiting");

                            FlickrLogic logic = new FlickrLogic(file.Item1);
                            await logic.Upload(file.Item2.Id);
                        }

                        TotalUpload--;
                    });
                });
        }

        public FlickrLogic(string path)
            : base(path)
        {
            FFileLogic fFileLogic = new FFileLogic(path);
            flickr = new Flickr();
        }

        public async Task<FFile> Upload(int fFileID)
        {
            while (IsNetworkOk == false)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }

            var file = fFileLogic.GetForSure(fFileID);
            var isExisted = fFileLogic.CheckExistHashCode(ExistFile, fFileID);

            if (isExisted)
            {
                if (file.Status == FFileStatus.New)
                    file.Status = FFileStatus.HashCodeFound;
                db.SaveChanges();
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
                    file.PhotoID = photoID;
                    file.Status = FFileStatus.Uploaded_NoSet;
                    file.UserID = Flickr.User.UserId;
                    db.SaveChanges();

                    UpdateSets(file.Id);

                    FlickrLogic.Log(file.Path, NoticeType.UploadDone, "Uploaded");
                }
            }

            return file;
        }

        public FFile UpdateSets(int fileID)
        {
            var file = db.FFiles.FirstOrDefault(r => r.Id == fileID);
            if (file != null && file.Status == FFileStatus.Uploaded_NoSet)
            {
                FlickrLogic.Log(file.Path, NoticeType.Upload, "Add to photo sets");

                SetLogic l = new SetLogic();
                string setID = l.AddPhoto(file);

                if (string.IsNullOrEmpty(setID))
                { }
                else
                {
                    file.SetsID = setID;
                    file.Status = FFileStatus.Uploaded_SyncSet;
                    db.SaveChanges();
                }
            }

            return file;
        }

        public FFile Processing_HashCodeFound(int fileID)
        {
            var file = fFileLogic.UpdateHashCode(fileID);

            if (file != null && file.Status == FFileStatus.HashCodeFound)
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

                        }
                        else
                        {
                            file.Status = FFileStatus.Uploaded_NoSet;
                            db.SaveChanges();

                            UpdateSets(file.Id);
                        }
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

        async static public Task StartUpload()
        {
            ResetCancellationToken();

            if (FlickrLogic.IsUpload)
            {

            }
            else
            {
                FlickrLogic.IsUpload = true;

                //wait for all upload complete if remaining
                while (TotalUpload > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                var currentFolderId = 0;
                while (true)
                {
                    if (CancellationToken != null)
                        CancellationToken.ThrowIfCancellationRequested();

                    //TODO: Error here. Should be fix. folder will be back loop
                    var folder = FFolderLogic.GetForCurrentUser(currentFolderId);
                    if (folder == null)
                    {
                        StopUpload();
                        //await Task.Delay(TimeSpan.FromSeconds(5));
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
