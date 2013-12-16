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
    public partial class FolderSelect : Form
    {
        public FolderSelect()
        {
            InitializeComponent();
            dataGridViewFolder.AutoGenerateColumns = false;
        }

        private void FolderSelect_Load(object sender, EventArgs e)
        {
            DataGridViewFolder_LoadData();
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog di = new FolderBrowserDialog();

            var r = di.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                FFolderLogic fL = new FFolderLogic();
                var v = fL.CreateIfNotExist(di.SelectedPath);

                FFileLogic ffL1 = new FFileLogic(v);
                ffL1.Add(new DirectoryInfo(v.Path));

                DataGridViewFolder_LoadData();
            }
        }

        void DataGridViewFolder_LoadData()
        {
            if (Flickr.User == null)
            {
                dataGridViewFolder.DataSource = null;

            }
            else
            {
                FSMasterDBContext db = new FSMasterDBContext();
                dataGridViewFolder.DataSource = db.FFolders.Where(r => r.UserId == Flickr.User.UserId).ToList();
            }

        }

        private void dataGridViewFolder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var r = MessageBox.Show("Delete! Sure?", "", MessageBoxButtons.OKCancel);

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                if (e.ColumnIndex == 0) //Assuming the button column as second column, if not can change the index
                {
                    var Id = (int)dataGridViewFolder.Rows[e.RowIndex].Cells[1].Value;
                    FFolderLogic fFolderLogic = new FFolderLogic();
                    fFolderLogic.Delete(Id);

                    DataGridViewFolder_LoadData();
                }
            }
        }
    }
}
