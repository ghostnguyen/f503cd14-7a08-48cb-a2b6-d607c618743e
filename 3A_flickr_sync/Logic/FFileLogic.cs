using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Models;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading;
using _3A_flickr_sync.FlickrNet;

namespace _3A_flickr_sync.Logic
{
    public class FFileLogic : FSDBLogic
    {
        public FFileLogic(string path)
            : base(path)
        {
        }

        private static ConcurrentQueue<Tuple<string, FFile>> fileList = new ConcurrentQueue<Tuple<string, FFile>>();

        public static int MinBuffer { get; set; }
        public static int Buffer { get; set; }


        static FFileLogic()
        {
            MinBuffer = FlickrLogic.MaxUpload * 2;
            Buffer = MinBuffer * 5;
        }

        public static Tuple<string, FFile> DequeueForUpload()
        {
            Tuple<string, FFile> f = null;
            if (fileList.TryDequeue(out f))
            { }
            else
            { f = null; }
            return f;
        }

        public static void EnqueueForUpload(Tuple<string, FFile> file)
        {
            if (file == null)
            {
            }
            else
            {
                var f = fileList.FirstOrDefault(r => r.Item1 == file.Item1 && r.Item2.Id == file.Item2.Id);
                if (f == null)
                {
                    fileList.Enqueue(file);
                }
            }
        }

        public static int QueueCount()
        {
            return fileList.Count;
        }


        static public void ClearForUpload()
        {
            fileList = new ConcurrentQueue<Tuple<string, FFile>>();
        }

        public FFileLogic(FFolder folder)
            : base(folder.Path)
        {
        }

        async public Task<bool> StartBuffer(CancellationToken cancellationToken)
        {
            Reset_ProcessingStatus();

            bool hasPhoto = true;
            var re = Observable.Interval(TimeSpan.FromSeconds(2)).TakeWhile(r => hasPhoto);

            re.Where(r => fileList.Count < MinBuffer)
                .Subscribe(r =>
                {
                    var l = TakeBuffer(Buffer);

                    if (l.Count == 0)
                    {
                        hasPhoto = false;
                    }
                    else
                    {
                        l.ForEach(r1 => fileList.Enqueue(new Tuple<string, FFile>(db.Fullpath, r1)));
                    }
                }, cancellationToken
                );

            await re;

            return hasPhoto;
        }

        public bool Add(FileInfo file)
        {
            bool isAdded = false;
            FFile v = null;
            if (file.Exists)
            {
                v = db.FFiles.FirstOrDefault(r => r.Path == file.FullName);

                if (v == null)
                {
                    db.FFiles.Add(new FFile() { Path = file.FullName, Status = FFileStatus.New });
                    db.SaveChanges();
                    isAdded = true;
                }
            }
            return isAdded;
        }

        public async Task Scan()
        {
            DirectoryInfo folder = new DirectoryInfo(SyncPath);
            if (folder.Exists)
            {
                await Task.Run(() =>
                    {
                        var ext = AppSetting.Extension;
                        int c = 0;
                        var f1 = Directory.EnumerateFiles(folder.FullName, "*.*", SearchOption.AllDirectories);

                        f1.ToObservable()
                            .Subscribe(r =>
                            {
                                var f = new FileInfo(r);
                                if (ext.Contains(f.Extension.ToLower()))
                                {
                                    if (Add(f))
                                    {
                                        c++;
                                        FlickrLogic.UploadEventList.Add(new Notice() { Type = NoticeType.AddFile, JobDone = c, FullPath = folder.FullName });
                                    }
                                }

                            },
                            () =>
                            {
                                FlickrLogic.UploadEventList.Add(new Notice() { Type = NoticeType.AddFileDone, JobDone = c, FullPath = folder.FullName });
                            })
                            ;
                    });
            }
        }

        public List<FFile> TakeBuffer(int count)
        {
            var r11 = db.FFiles.ToList();

            var r = db.FFiles
                .Where(r1 => true
                    && (r1.Status == FFileStatus.New || r1.Status == FFileStatus.Existing)
                    && r1.Path.Contains(db.Fullpath)
                    && r1.ProcessingStatus == null
                    )
                .Take(count)
                .ToList();

            r.ForEach(r1 => r1.ProcessingStatus = ProcessingStatus.Processing);

            db.SaveChanges();

            return r;
        }

        public bool CheckExistHashCode(Func<string, bool> flickrCheckF, int fFileID)
        {
            UpdateHashCode(fFileID);

            return DoUpdate(fFileID, file =>
            {
                FlickrLogic.Log(file.Path, NoticeType.Upload, "Check existing");

                bool check = flickrCheckF(file.HashCode);

                if (check)
                    FlickrLogic.Log(file.Path, NoticeType.UploadDone, "Existed");

                return check;
            }, false);
        }

        public FFile GetForSure(int fFileID)
        {
            var file = Get(fFileID);

            if (file == null)
            {
            }
            else
            {
                FileInfo fileInfo = new FileInfo(file.Path);
                if (fileInfo.Exists)
                { }
                else { file = null; }
            }

            return file;
        }

        public FFile Get(int fFileID)
        {
            var file = db.FFiles.Find(fFileID);

            if (file != null && file.Path.Contains(db.Fullpath))
            {
            }
            else { file = null; }

            return file;
        }

        public FFile UpdateHashCode(int fFileID)
        {
            return DoUpdate(fFileID, file =>
            {
                var hashCode = Helper.HashFile(file.Path);
                var hashCodeNoExif = Helper.HashPhotoNoExif(file.Path);

                if (string.IsNullOrEmpty(hashCode))
                {
                    file = null;
                }
                else
                {
                    if (string.IsNullOrEmpty(file.HashCode))
                    {
                        file.HashCode = hashCode;
                        file.HashCodeNoExif = hashCodeNoExif;
                    }
                }
            });
        }

        public FFile UpdateFlickrIds(int fFileID, Func<string, PhotoCollection> getPhoto_ByHashCode)
        {
            return DoUpdate(fFileID, file =>
            {
                if (string.IsNullOrEmpty(file.PhotoID))
                {
                    var l = getPhoto_ByHashCode(file.HashCode);
                    if (l.Count() > 0)
                    {
                        var photo = l.First();
                        file.PhotoID = photo.PhotoId;
                        file.UserID = photo.UserId;
                    }
                    else
                    {
                        file.PhotoID = null;
                        file.SetsID = null;
                        file.UserID = null;
                    }
                }
            });
        }

        public FFile UpdateStatus(int fFileID)
        {
            return DoUpdate(fFileID, file =>
            {
                if (string.IsNullOrEmpty(file.PhotoID))
                {
                    file.Status = FFileStatus.New;
                }
            });
        }

        public T DoUpdate<T>(int fFileID, Func<FFile, T> fUpdate, bool isSaveChanges = true)
        {
            T r = default(T);
            var file = Get(fFileID);

            if (file != null)
            {
                r = fUpdate(file);

                if (isSaveChanges)
                    db.SaveChanges();
            }

            return r;
        }

        public FFile DoUpdate(int fFileID, Action<FFile> fUpdate, bool isSaveChanges = true)
        {
            var file = Get(fFileID);

            if (file != null)
            {
                fUpdate(file);

                if (isSaveChanges)
                    db.SaveChanges();
            }

            return file;
        }

        public void Reset_ProcessingStatus()
        {
            var v = db.FFiles.Where(r => r.ProcessingStatus != null);

            foreach (var item in v)
            {
                item.ProcessingStatus = null;
            }
            db.SaveChanges();
        }
    }
}
