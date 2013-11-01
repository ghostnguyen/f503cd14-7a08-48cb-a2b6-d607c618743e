using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            //FFolderLogic fL = new FFolderLogic();
            //var v = fL.CreateIfNotExist(@"D:\ghostnguyen\Pictures\SGI Photo - Can Gio");

            //FFileLogic ffL1 = new FFileLogic(v);
            //ffL1.Add(new DirectoryInfo(v.Path));            

            //FUserLogic fUserLogic = new FUserLogic();
            //fUserLogic.CreateOrUpdate("22355115@N04", "72157637144747383-7edb312576a89e3f", "0095950bcad5ee34");

            //Flickr f = new Flickr();
            //var v1 = f.TestLogin();

            //f.UploadPicture(@"D:\ghostnguyen\Pictures\SGI Photo - Can Gio\DSC03488.JPG");

            //var s1 = Helper.HashFile(@"D:\ghostnguyen\Pictures\SGI Photo - Can Gio\DSC03488.JPG");
            //var s2 = Helper.HashPhotoNoExif(@"D:\ghostnguyen\Pictures\SGI Photo - Can Gio\DSC03488.JPG");

            //var s3 = Helper.HashFile(@"D:\ghostnguyen\Downloads\10588108734_8fd7ee6f1a_o.jpg");
            //var s4 = Helper.HashPhotoNoExif(@"D:\ghostnguyen\Downloads\10588108734_8fd7ee6f1a_o.jpg");

        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //How to get token for user 
            //Flickr f = new Flickr();
            //var r = f.OAuthGetRequestToken("oob");
            //Token = "72157637144674393-a524ed4dd4f6fe99"
            //TokenSecret = "a7f980526e160420"
            //Go to: http://www.flickr.com/services/oauth/authorize?oauth_token=72157637144674393-a524ed4dd4f6fe99
            //var v = f.OAuthGetAccessToken(r, "283-464-080");
            //OAuthAccessToken = "72157637144747383-7edb312576a89e3f";
            //OAuthAccessTokenSecret = "0095950bcad5ee34";


        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
