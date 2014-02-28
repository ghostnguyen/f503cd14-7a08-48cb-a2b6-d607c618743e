namespace _3A_flickr_sync
{
    partial class Download
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnFolder = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnDownload = new System.Windows.Forms.Button();
            this.setsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._3A_Flickr_Sync_Master_DBDataSet = new _3A_flickr_sync._3A_Flickr_Sync_Master_DBDataSet();
            this.setsTableAdapter = new _3A_flickr_sync._3A_Flickr_Sync_Master_DBDataSetTableAdapters.SetsTableAdapter();
            this.lblDownloadPath = new System.Windows.Forms.Label();
            this.setsIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tittleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isDownloadDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._3A_Flickr_Sync_Master_DBDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(12, 12);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(100, 23);
            this.btnFolder.TabIndex = 0;
            this.btnFolder.Text = "Save to folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.setsIDDataGridViewTextBoxColumn,
            this.userIDDataGridViewTextBoxColumn,
            this.tittleDataGridViewTextBoxColumn,
            this.pathDataGridViewTextBoxColumn,
            this.isDownloadDataGridViewCheckBoxColumn});
            this.dataGridView1.DataSource = this.setsBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(563, 302);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownload.Location = new System.Drawing.Point(500, 349);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // setsBindingSource
            // 
            this.setsBindingSource.DataMember = "Sets";
            this.setsBindingSource.DataSource = this._3A_Flickr_Sync_Master_DBDataSet;
            // 
            // _3A_Flickr_Sync_Master_DBDataSet
            // 
            this._3A_Flickr_Sync_Master_DBDataSet.DataSetName = "_3A_Flickr_Sync_Master_DBDataSet";
            this._3A_Flickr_Sync_Master_DBDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // setsTableAdapter
            // 
            this.setsTableAdapter.ClearBeforeFill = true;
            // 
            // lblDownloadPath
            // 
            this.lblDownloadPath.AutoSize = true;
            this.lblDownloadPath.Location = new System.Drawing.Point(118, 17);
            this.lblDownloadPath.Name = "lblDownloadPath";
            this.lblDownloadPath.Size = new System.Drawing.Size(35, 13);
            this.lblDownloadPath.TabIndex = 3;
            this.lblDownloadPath.Text = "label1";
            // 
            // setsIDDataGridViewTextBoxColumn
            // 
            this.setsIDDataGridViewTextBoxColumn.DataPropertyName = "SetsID";
            this.setsIDDataGridViewTextBoxColumn.HeaderText = "SetsID";
            this.setsIDDataGridViewTextBoxColumn.Name = "setsIDDataGridViewTextBoxColumn";
            this.setsIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // userIDDataGridViewTextBoxColumn
            // 
            this.userIDDataGridViewTextBoxColumn.DataPropertyName = "UserID";
            this.userIDDataGridViewTextBoxColumn.HeaderText = "UserID";
            this.userIDDataGridViewTextBoxColumn.Name = "userIDDataGridViewTextBoxColumn";
            this.userIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // tittleDataGridViewTextBoxColumn
            // 
            this.tittleDataGridViewTextBoxColumn.DataPropertyName = "Tittle";
            this.tittleDataGridViewTextBoxColumn.HeaderText = "Tittle";
            this.tittleDataGridViewTextBoxColumn.Name = "tittleDataGridViewTextBoxColumn";
            this.tittleDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pathDataGridViewTextBoxColumn
            // 
            this.pathDataGridViewTextBoxColumn.DataPropertyName = "Path";
            this.pathDataGridViewTextBoxColumn.HeaderText = "Path";
            this.pathDataGridViewTextBoxColumn.Name = "pathDataGridViewTextBoxColumn";
            this.pathDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // isDownloadDataGridViewCheckBoxColumn
            // 
            this.isDownloadDataGridViewCheckBoxColumn.DataPropertyName = "IsDownload";
            this.isDownloadDataGridViewCheckBoxColumn.HeaderText = "IsDownload";
            this.isDownloadDataGridViewCheckBoxColumn.Name = "isDownloadDataGridViewCheckBoxColumn";
            // 
            // Download
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 384);
            this.Controls.Add(this.lblDownloadPath);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnFolder);
            this.Name = "Download";
            this.Text = "Download";
            this.Load += new System.EventHandler(this.Download_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._3A_Flickr_Sync_Master_DBDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnDownload;
        private _3A_Flickr_Sync_Master_DBDataSet _3A_Flickr_Sync_Master_DBDataSet;
        private System.Windows.Forms.BindingSource setsBindingSource;
        private _3A_Flickr_Sync_Master_DBDataSetTableAdapters.SetsTableAdapter setsTableAdapter;
        private System.Windows.Forms.Label lblDownloadPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn setsIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tittleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isDownloadDataGridViewCheckBoxColumn;
    }
}