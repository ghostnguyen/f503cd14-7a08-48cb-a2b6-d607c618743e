using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.FlickrNet;
using _3A_flickr_sync.Logic;
using _3A_flickr_sync.Models;
using System.Data.Entity;
using _3A_flickr_sync.Migrations;
using System.Data.Entity.Migrations;

namespace _3A_flickr_sync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<FSMasterDBContext, Configuration>());

            var dbMigrator = new DbMigrator(new Configuration());
            dbMigrator.Update();

            //FSMasterDBContext c = new FSMasterDBContext();
            //c.Database.CreateIfNotExists();

            System.Net.ServicePointManager.DefaultConnectionLimit = 10;

            Flickr.ResetOAuth();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.UnhandledException += currentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            FlickrLogic.Log("", NoticeType.UploadException, ex.Message);
        }
    }
}
