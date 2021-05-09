using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace CropImage
{
    [Activity(Label = "AzureActivity")]
    public class AzureActivity : Activity
    {
        AzureListAdapter listAdapter;
        public static string myuseridazure;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.AzureListActivity);
            listAdapter = new AzureListAdapter(this);
            var listView = FindViewById<ListView>(Resource.Id.AzurelistView);
            //Hook up our adapter to our ListView
            listView.Adapter = listAdapter;
            //Empty table Mycheckbox before select to delete
            ISQLiteConnection connactionc = null;
            ISQLiteConnectionFactory factoryc = new MvxDroidSQLiteConnectionFactory();

            var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filename = sqlitedir.Path + "/mysqliteaction.db";
            //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();


        }

    }
}