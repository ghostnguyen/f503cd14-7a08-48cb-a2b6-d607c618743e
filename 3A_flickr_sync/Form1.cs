using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3A_flickr_sync.Models;

namespace _3A_flickr_sync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FSDBContext db = new FSDBContext();
            db.Folders.Add(new Folder() { Path = "ada"});
            db.SaveChangesAsync();
        }
    }
}
