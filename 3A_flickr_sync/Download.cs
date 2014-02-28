using _3A_flickr_sync.Common;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Logic;
using _3A_flickr_sync.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3A_flickr_sync
{
    public partial class Download : Form
    {
        public Download()
        {
            InitializeComponent();
            this.setsTableAdapter.Connection.ConnectionString = FSMasterDBContext.GetConnectionString();

            FUserLogic l = new FUserLogic();
            var v =l.GetFirst();
            if (v == null) { }
            else { lblDownloadPath.Text = v.DownloadPath; }
        }

        private void Download_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the '_3A_Flickr_Sync_Master_DBDataSet.Sets' table. You can move, or remove it, as needed.
            //this.setsTableAdapter.Fill(this._3A_Flickr_Sync_Master_DBDataSet.Sets);

            this.setsTableAdapter.FillBy(this._3A_Flickr_Sync_Master_DBDataSet.Sets, Flickr.User.UserId);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var g = sender as DataGridView;

            var name = g.Columns[e.ColumnIndex].DataPropertyName;

            if (name == "IsDownload")
            {
                var idCell = g.Rows[e.RowIndex].Cells[0];
                var id = (string)idCell.Value;
                var isDownload = (bool)g.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;

                SetLogic l = new SetLogic();
                l.UpdateIsDownload(id, isDownload);
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog di = new FolderBrowserDialog();

            var r = di.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                FUserLogic l = new FUserLogic();
                var v = l.Update(di.SelectedPath);
                if (v == null)
                { }
                else
                {
                    lblDownloadPath.Text = v.DownloadPath;
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

        }

    }
}
