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
    public partial class Enter_Oauth_Verifier : Form
    {
        public Enter_Oauth_Verifier()
        {
            InitializeComponent();
        }

        public string Code
        {
            get
            {
                return txtCode.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
