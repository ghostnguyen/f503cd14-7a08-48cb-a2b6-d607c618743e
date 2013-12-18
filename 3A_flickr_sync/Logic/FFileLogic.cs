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
            bool hasPhoto = true;
            var re = Observable.Interval(TimeSpan.FromSeconds(2)).TakeWhile(r => hasPhoto);

            re.Where(r => fileList.Count < MinBuffer)
                .Subscribe(r =>
                {
                    var last = fileList.LastOrDefault(r1 => r1.Item1 == db.Path);

                    int fromID = last == null ? 0 : last.Item2.Id;

                    while (fromID == 0 && FlickrLogic.TotalUpload > 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

                    var l = TakeNew(Buffer, fromID);

                    if (l.Count == 0)
                    {
                        hasPhoto = false;
                    }
                    else
                    {
                        l.ForEach(r1 => fileList.Enqueue(new Tuple<string, FFile>(db.Path, r1)));
                    }
                }, cancellationToken
                );

            await re;

            return hasPhoto;
        }

        public FFile Add(FileInfo file)
        {
            FFile v = null;
            if (file.Exists)
            {
                v = db.FFiles.Where(r => r.Path == file.FullName).FirstOrDefault();

                if (v == null)
                {
                    db.FFiles.Add(new FFile() { Path = file.FullName, Status = FFileStatus.New });
                    db.SaveChanges();
                }
            }
            return v;
        }

        public void Add(DirectoryInfo folder)
        {
            if (folder.Exists)
            {
                Task.Run(() =>
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
                                    Add(f);
                                    c++;
                                    FlickrLogic.UploadEventList.Add(new Notice() { Type = NoticeType.AddFile, JobDone = c, FullPath = folder.FullName });
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

        public List<FFile> TakeNew(int count, int fromID = 0)
        {
            return db.FFiles
                .Where(r1 => r1.Status == FFileStatus.New && r1.Path.Contains(db.Path)
                && r1.Id > fromID)
                .Take(count)
                .ToList();
        }
    }
}
