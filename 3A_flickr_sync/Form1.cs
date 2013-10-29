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
        }
    }
}
