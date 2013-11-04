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

namespace _3A_flickr_sync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            FFolderLogic fL = new FFolderLogic();
            var v = fL.CreateIfNotExist(@"D:\ghostnguyen\Pictures\SGI Photo - Can Gio");

            FFileLogic ffL1 = new FFileLogic(v);
            ffL1.Add(new DirectoryInfo(v.Path));

            FlickrLogic fLg = new FlickrLogic(v.Path);
            fLg.Upload();
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
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
