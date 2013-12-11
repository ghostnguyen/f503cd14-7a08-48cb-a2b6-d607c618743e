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

namespace _3A_flickr_sync
{
    public partial class Main : Form
    {
        

        public Main()
        {
            InitializeComponent();

            

        }

        void LoadGUIByUser()
        {
            bool hasUser = Flickr.User != null;

            selectFoldersToolStripMenuItem.Visible = hasUser;
            startUploadToolStripMenuItem.Visible = hasUser;
            clearLogToolStripMenuItem.Visible = hasUser;

            rtbLog.Clear();
            rtbProgress.Clear();

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

        private void Main_Load(object sender, EventArgs e)
        {
            LoadGUIByUser();

            Observable.Interval(TimeSpan.FromSeconds(1))
                .Where(r => FlickrLogic.UploadEventList.IsEmpty == false)
                .ObserveOn(rtbProgress)
                .Subscribe(r =>
                {
                    Notice n;
                    if (FlickrLogic.UploadEventList.TryDequeue(out n))
                    {
                        var l = rtbProgress.Lines.ToList();
                        var currentLine = l.Find(r1 => r1.Contains(n.FullPath));

                        var index = l.IndexOf(currentLine);

                        if (n.Ex == null)
                        {
                            if (n.UploadProgress.ProgressPercentage == 100)
                            {
                                if (index == -1)
                                { }
                                else
                                {
                                    l.RemoveAt(index);
                                    rtbProgress.Lines = l.ToArray();
                                }

                                rtbLog.Text.Insert(0, n.FullPath + ": Completed \n");
                            }
                            else
                            {
                                string log = string.Format("{0}: {1}% ({2}/{3})", n.FullPath, n.UploadProgress.ProgressPercentage, n.UploadProgress.BytesSent / 1024, n.UploadProgress.TotalBytesToSend / 1024);
                                if (index == -1)
                                {
                                    rtbProgress.AppendText(log);
                                }
                                else
                                {
                                    l[index] = log;
                                    var la = l.ToArray();
                                    rtbProgress.Lines = la;
                                }
                            }
                        }
                        else
                        {
                            if (index == -1)
                            { }
                            else
                            {
                                l.RemoveAt(index);
                                rtbProgress.Lines = l.ToArray();
                            }

                            rtbLog.Text.Insert(0, string.Format("{0}: {1} \n", n.FullPath, n.Ex.Message));
                        }
                    }
                }
            );
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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            rtbLog.Clear();
            rtbProgress.Clear();
        }
    }
}
