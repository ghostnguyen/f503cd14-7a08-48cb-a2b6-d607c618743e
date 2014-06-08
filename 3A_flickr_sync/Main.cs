using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Logic;
using _3A_flickr_sync.Models;
using System.Threading;
using System.Reactive.Linq;
using _3A_flickr_sync.Properties;
using System.Collections.Specialized;

namespace _3A_flickr_sync
{
    public partial class Main : Form
    {
        List<Notice> noteToDisplay = new List<Notice>();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadGUIByUser();

            var noticeObserable = FlickrLogic.UploadEventList.ObservesChanged()
                .Where(r => r.EventArgs.Action == NotifyCollectionChangedAction.Add)
                .Select(r => r.EventArgs.NewItems.Cast<Notice>().ToList())
                .Buffer(TimeSpan.FromSeconds(0.5))
                .Select(r => r.SelectMany(r1 => r1))
                .Where(r => r.Count() > 0)
                .ObserveOn(this);

            noticeObserable.Subscribe(r =>
                {
                    FlickrLogic.UploadEventList.Clear();

                    r.Where(r1 => r1.Type == NoticeType.Upload || r1.Type == NoticeType.AddFile)
                        .GroupBy(r1 => new { r1.Type, r1.FullPath })
                        .Select(r1 => r1.OrderByDescending(r2 => r2.Datetime).First())
                        .ToList()
                        .ForEach(r1 =>
                            {
                                var n = noteToDisplay.Find(r2 => r2.Type == r1.Type && r2.FullPath == r1.FullPath);
                                if (n == null)
                                {
                                    noteToDisplay.Add(r1);
                                }
                                else
                                {
                                    n.Percentage = r1.Percentage;
                                    n.JobDone = r1.JobDone;
                                    n.JobTotal = r1.JobTotal;
                                    n.Note = r1.Note;
                                }
                            }
                        )
                    ;

                    r.Where(r1 => r1.Type != NoticeType.Upload && r1.Type != NoticeType.AddFile)
                        .ToList()
                        .ForEach(r1 =>
                        {
                            rtbLog.InsertLineAtFirst(r1.GetNote());
                        });

                    r.Where(r1 => r1.Type == NoticeType.UploadDone || r1.Type == NoticeType.AddFileDone || r1.Type == NoticeType.DownloadDone)
                        .ToList()
                        .ForEach(r1 =>
                        {
                            noteToDisplay.RemoveAll(r2 => r2.FullPath == r1.FullPath);
                        });

                    dataGridViewNote_ShowNote();
                })
                ;

            FlickrLogic.IsUploadNotify.ObserveOn(this).Subscribe(r =>
            {
                var menu = startUploadToolStripMenuItem;

                if (r)
                {
                    menu.Image = Resources.Uploading;
                    menu.Text = "Stop";
                }
                else
                {
                    menu.Image = Resources.Flickr;
                    menu.Text = "Start upload";
                }
            });
        }

        void LoadGUIByUser()
        {
            bool hasUser = Flickr.User != null;

            selectFoldersToolStripMenuItem.Visible = hasUser;

            startUploadToolStripMenuItem.Visible = hasUser;
            startUploadToolStripMenuItem.Image = Resources.Flickr;

            clearLogToolStripMenuItem.Visible = hasUser;
            downloadToolStripMenuItem.Visible = hasUser;

            Clear();

            if (hasUser)
            {
                this.Text = "3A Flickr Sync - " + Flickr.User.UserName;
                loginToolStripMenuItem.Text = "Login to other account ";
            }
            else
            {
                loginToolStripMenuItem.Text = "Login";
            }
        }



        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //How to get token for user 
            Flickr f = new Flickr();
            var r = f.OAuthGetRequestToken("oob");
            //Token = "72157637144674393-a524ed4dd4f6fe99"
            //TokenSecret = "a7f980526e160420"
            //Go to: http://www.flickr.com/services/oauth/authorize?oauth_token=72157637144674393-a524ed4dd4f6fe99

            string url = @"http://www.flickr.com/services/oauth/authorize?oauth_token=" + r.Token;
            ProcessStartInfo sInfo = new ProcessStartInfo(url);
            Process.Start(sInfo);

            Enter_Oauth_Verifier f1 = new Enter_Oauth_Verifier();
            f1.ShowDialog();
            var code = f1.Code;
            if (string.IsNullOrEmpty(code))
            {

            }
            else
            {
                //var v = f.OAuthGetAccessToken(r, "283-464-080");
                //OAuthAccessToken = "72157637144747383-7edb312576a89e3f";
                //OAuthAccessTokenSecret = "0095950bcad5ee34";

                var v = f.OAuthGetAccessToken(r, code);
                if (v != null)
                {
                    FUserLogic lo = new FUserLogic();
                    lo.CreateOrUpdate(v.UserId, v.Token, v.TokenSecret, v.Username, v.FullName);

                    //cancel any uploading process
                    FlickrLogic.CancellationTokenSrc.Cancel();

                    Flickr.ResetOAuth();
                    LoadGUIByUser();
                }
            }
        }

        private void selectFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderSelect form = new FolderSelect();
            form.ShowDialog();
        }



        private void startUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlickrLogic.IsUpload = !FlickrLogic.IsUpload;
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            rtbLog.Clear();
            rtbNote.Clear();
            noteToDisplay = new List<Notice>() { };

            //Can not set datasource has zero item to dataGridView. It should to be set to null
            //dataGridViewNote.DataSource = null;

        }
        void dataGridViewNote_ShowNote()
        {
            rtbNote.Lines = noteToDisplay.Select(r => r.GetNote()).ToArray();
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Download frm = new Download();
            frm.ShowDialog();
        }

    }
}
