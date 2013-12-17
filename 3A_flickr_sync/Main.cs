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
                .Buffer(TimeSpan.FromSeconds(1))
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
                                }
                            }
                        )
                    ;

                    dataGridViewNote_ShowNote();

                    r.Where(r1 => r1.Type != NoticeType.Upload && r1.Type != NoticeType.AddFile)
                        .ToList()
                        .ForEach(r1 =>
                        {
                            rtbLog.InsertLineAtFirst(r1.GetNote());
                        });
                })
                ;

            //noteToDisplay.Add(new Notice());
            //noteToDisplay.Add(new Notice());
            //noteToDisplay.Add(new Notice());
            //dataGridViewNote_ShowNote();
        }

        void LoadGUIByUser()
        {
            bool hasUser = Flickr.User != null;

            selectFoldersToolStripMenuItem.Visible = hasUser;
            startUploadToolStripMenuItem.Visible = hasUser;
            clearLogToolStripMenuItem.Visible = hasUser;

            Clear();

            if (hasUser)
            {
                loginToolStripMenuItem.Text = string.Format("Current user: {0}. Click to login to other account.", Flickr.User.UserName);
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
            var menu = sender as ToolStripMenuItem;

            if (FlickrLogic.IsUpload)
            {
                menu.Image = null;
                menu.Text = "Start upload";
                FlickrLogic.StopUpload();
            }
            else
            {
                menu.Image = Resources.Uploading;
                menu.Text = "Stop";
                Task.Run(() => FlickrLogic.StartUpload());
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlickrLogic.UploadEventList.Add(new Notice() { FullPath = DateTime.Now.ToString() });
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            rtbLog.Clear();
            noteToDisplay = new List<Notice>() { };

            //Can not set datasource has zero item to dataGridView. It should to be set to null
            dataGridViewNote.DataSource = null;
        }
        void dataGridViewNote_ShowNote()
        {
            if (noteToDisplay == null)
            {
            }
            else
            {
                noteToDisplay.Where(r => r.Percentage == 100f)
                    .ToList()
                    .ForEach(r =>
                        rtbLog.InsertLineAtFirst(r.GetNote())
                    );

                noteToDisplay.RemoveAll(r => r.Percentage == 100f);
            }

            dataGridViewNote.ClearSelection();
            if (noteToDisplay.Count == 0)
            {
                //Can not set datasource has zero item to dataGridView. It should to be set to null
                dataGridViewNote.DataSource = null;
            }
            else
            {
                dataGridViewNote.DataSource = noteToDisplay;
            }
            
        }

    }
}
