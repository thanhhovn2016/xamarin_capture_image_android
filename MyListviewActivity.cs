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
using Android.Graphics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;

using System.IO;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid;
using System.Net;


namespace CropImage
{
    [Activity(Label = "MyListviewActivity")]
    public class MyListviewActivity : Activity
  {
    CustomListAdapter listAdapter;
    int count = 1;

    //public static string sas = "https://thanhhosr.blob.core.windows.net/teal-test?sv=2014-02-14&sr=c&sig=vmhkScl0%2Fdji1VfXFltJe2asrf%2FAiF%2BF%2Bdia%2BtpBkmk%3D&se=2116-02-29T23%3A29%3A11Z&sp=rwdl";
    public static string sas = "https://thanhhosr.blob.core.windows.net/thanhgg?se=2016-03-03T02%3A56%3A42Z&sp=rwdl&sv=2014-02-14&sr=c&sig=oeX9AJoVwQUGJJjGRZFcB6U6nlSmNwUxJe2xrsCOH20%3D";
    
    static string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=thanhhosr;AccountKey=4WrY+PtDhmwn4egEY8ti9dEtHrn/gzNqjbPYU7UCOZv+0+AfNbyq9zXtVEW1RigrPUcz6iXsxx8B7oqNN5DbzQ==";
    public class Myimage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Imagepath { get; set; }
    }
    public static string useridconn;
    public static string usertypeconn;
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);
      //Set the Activity's view to our list layout    
      SetContentView(Resource.Layout.ListActivity);
        //sqlite for sas azure

      ISQLiteConnection conn = null;

      ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

      var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
      string filenameaction = sqlitedir.Path + "/mysqlitesas.db";
      string filenameactionazure = sqlitedir.Path + "/mysqlitesasazure.db";

      conn = factory.Create(filenameaction);
      conn.CreateTable<Azurecon>();
      var myuserid = "";
      foreach (var e in conn.Table<Azurecon>())
      {
          myuserid = e.UserId;
      }
      conn.Close();

      //sqlite for sas azure end

      //Create our adapter
      listAdapter = new CustomListAdapter(this);
      //Find the listview reference
      var listView = FindViewById<ListView>(Resource.Id.listView);
      //Hook up our adapter to our ListView
      listView.Adapter = listAdapter;
      //Wire up the click event
      //listView.ItemClick += new EventHandler<ItemEventArgs>(listView_ItemClick);
        Button deleteselectedfiles = FindViewById<Button>(Resource.Id.deleteselectfiles);
        deleteselectedfiles.Click += async delegate
        {
            //Delete files in sqlite table MyImage and in sdcard
            var myfiledir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                ISQLiteConnection conncf = null;
                ISQLiteConnection connactiondf = null;
                ISQLiteConnectionFactory factorydf = new MvxDroidSQLiteConnectionFactory();

                
                string sqlfilenamed = sqlitedir.Path + "/mysqliteaction.db";
                //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();

                connactiondf = factorydf.Create(sqlfilenamed);
                connactiondf.CreateTable<MyCheckbox>();

                List<string> myCollection = new List<string>();
                var countidx = 0;
                HashSet<string> myimgspath = new HashSet<string>();
                foreach (var e in connactiondf.Table<MyCheckbox>())
                {
                    string imgfilename = "file://" + sqlitedir.Path + "/" + e.Name;
                    myimgspath.Add(imgfilename);
                    var myfilepath = myfiledir + "/" + e.Name;
                    if (File.Exists(myfilepath))
                        File.Delete(myfilepath);
                }
                connactiondf.Close();

                List<string> myimglistdeletecmd = new List<string>();
                var myvaridx = 0;            
                foreach(string permyimg in myimgspath){
                    var myquerycmd = "Delete from Myimage where Imagepath = '" + permyimg + "'";
                    myimglistdeletecmd.Add(myquerycmd);
 

                }
                ISQLiteConnection connactioncf = null;

                ISQLiteConnectionFactory factorycf = new MvxDroidSQLiteConnectionFactory();
                var sqlitedirc = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                string filenameactionc = sqlitedirc.Path + "/mysqliteimage.db";
                connactioncf = factorycf.Create(filenameactionc);
                connactioncf.CreateTable<Myimage>();
                
         
                //var permyimgfile = new Myimage(){Date = "30-12-2016",Imagepath = myimgfile};
                    
                //myconn.CreateCommand("Delete from Myimage where Imagepath ='" + myimgfile + "'");
                //myconn.Dispose();
                //myconn.Commit();
                //connactioncf.CreateCommand("Delete from Myimage where Imagepath ='" + "file:///storage/emulated/0/Pictures/Boruto/myPhoto_69d38ce2-0a96-41ed-884d-021a24890f88.jpg" + "'").ExecuteNonQuery();
            foreach (var cmd in myimglistdeletecmd) {
                connactioncf.CreateCommand(cmd).ExecuteNonQuery();
            }
            
            connactioncf.Commit();
            connactioncf.Close();
            



        };
        Button uploadtoazure = FindViewById<Button>(Resource.Id.uploadtoazure);
        uploadtoazure.Click += async delegate
        {

            uploadtoazure.Text = string.Format("{0} clicks!", count++);
            //Get userid from sqlite db file
            
            ISQLiteConnection connacc = null;

            ISQLiteConnectionFactory factoryacc = new MvxDroidSQLiteConnectionFactory();

            var sqlitediracc = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filenameactionacc = sqlitediracc.Path + "/mysqlitesas.db";

            connacc = factoryacc.Create(filenameactionacc);
            connacc.CreateTable<Azurecon>();
            var useridconnc = "";
            foreach (var e in connacc.Table<Azurecon>())
            {
                useridconnc = e.UserId;
            }
            connacc.Close();
            //myuserid = "115708452302383620142";

            var myurl = "http://93.118.34.239:8888/" + useridconnc;
            Uri azureuri = new Uri(myurl);
            
            
            HttpWebRequest request = new HttpWebRequest(azureuri);
            request.Method = "GET";


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {

                string responseString = sr.ReadToEnd();
                Toast.MakeText(this, "Connect SAS String:" + responseString, ToastLength.Short).Show();
                try
                {
                    await UseContainerSAS(responseString);
                }
                catch { 
                }
                

            }
             
            

        };
   
    }


   



    static async Task UseContainerSAS(string sas)
    {
        //Toast toast1 = Toast.MakeText(Application.Context, "BBBBBBBBBBBBBBBBBBBBBBBBB", ToastLength.Short);
        //toast1.Show();
        //Try performing container operations with the SAS provided.



        //Return a reference to the container using the SAS URI.
        //CloudStorageAccount account;
        CloudBlobContainer container = new CloudBlobContainer(new Uri(sas));
        string date = DateTime.Now.ToString();
        try
        {
            try
            {

                //MemoryStream memoryStream = new MemoryStream();
                var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                //string filename = sqlitedir.Path + "/myPhoto_3c2e2adb-b374-466b-b32d-b1a2fdea65a0.jpg";
                ISQLiteConnection connactionc = null;
                ISQLiteConnectionFactory factoryc = new MvxDroidSQLiteConnectionFactory();                
                string sqlfilename = sqlitedir.Path + "/mysqliteaction.db";
                //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();

                connactionc = factoryc.Create(sqlfilename);
                connactionc.CreateTable<MyCheckbox>();

                List<string> myCollection = new List<string>();
                var countidx = 0;
                foreach (var e in connactionc.Table<MyCheckbox>())
                {
                    string imgfilename = sqlitedir.Path + "/" + e.Name;
                    myCollection.Add(imgfilename);
                    Toast.MakeText(Application.Context, imgfilename, ToastLength.Short).Show();

                    using (FileStream fileStream = new FileStream(imgfilename, FileMode.Open, FileAccess.Read))
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        CloudBlockBlob blob = container.GetBlockBlobReference(e.Name);
                        fileStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        Toast.MakeText(Application.Context, memoryStream.Length.ToString(), ToastLength.Short).Show();


                        using (memoryStream)
                        {
                            await blob.UploadFromStreamAsync(memoryStream);
                            Toast toast = Toast.MakeText(Application.Context, "Upload Complete", ToastLength.Short);
                            toast.Show();
                        }
                    }
                    countidx++;
                }
                connactionc.Close();                 
            }

            catch (Java.Lang.Exception e)
            {

            }
        }
        catch (Java.Lang.Exception eerror)
        { }

    }

    void listView_ItemClick(object sender, ItemEventArgs e)
    {
      //Get our item from the list adapter
      var item = this.listAdapter.GetItemAtPosition(e.Position);
      //Make a toast with the item name just to show it was clicked
      Toast.MakeText(this, item.Name + " Clicked!", ToastLength.Short).Show();
    }

  }
}