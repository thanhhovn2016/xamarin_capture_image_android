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
using Android.Graphics;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using System.IO;

using Android.Provider;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;
using Microsoft.WindowsAzure.Storage.Blob;
using Android.Webkit;
namespace CropImage
{

    public class AzureListAdapter : BaseAdapter
    {


        public static string myuseridazure;
        private Java.IO.File createDirectoryForPictures()
        {
            var dir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            return dir;
        }
        public class Myimage
        {
            public string Date { get; set; }
            public string Imagepath { get; set; }
        }
        Activity context;
        public List<Animal> items;
        public List<MyCheckbox> mycbitems;
        public AzureListAdapter(Activity context) //We need a context to inflate our row view from
            : base()
        {
            this.context = context;
            
            //Get list Blob Images from Azure host
            //Get userid from sqlite
            ISQLiteConnection conn = null;

            ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

            var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filenameaction = sqlitedir.Path + "/mysqlitesas.db";
            conn = factory.Create(filenameaction);
            conn.CreateTable<Azurecon>();
            foreach (var e in conn.Table<Azurecon>())
            {
                myuseridazure = e.UserId;
            }
            //Toast.MakeText(this, "Your Google UserId:" + myuseridazure, ToastLength.Short).Show();
            conn.Close();
            //var myurl = "http://93.118.34.239:8888/listblobs/" + myuseridazure;
            var myurl = "http://93.118.34.239:8888/listblobs/115708452302383620142";

            Uri azureuri = new Uri(myurl);
            var myanimallist = new List<Animal>();
            //Toast.MakeText(this, "Connect SAS String:" + myurl, ToastLength.Short).Show();            
            /*
            HttpWebRequest request = new HttpWebRequest(azureuri);
            request.Method = "GET";


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            var myanimallist = new List<Animal>();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {

                string responseString = sr.ReadToEnd();

                JArray bloburlarr = JArray.Parse(responseString);
                var countidx = 0;
                foreach (var perlurl in bloburlarr)
                {
                    var newanialele = new Animal() { Name = "Test Azure", Description = perlurl["imgname"].ToString(), Image = "Test Path", Mycheckbox = countidx };
                    countidx++;
                    myanimallist.Add(newanialele);
                }



                this.items = myanimallist;
            }
            */
            string auzremainurl = "https://spc.blob.core.windows.net/";
            string usercontainer = "";
            //Get SAS connection string

            ISQLiteConnection connacc = null;

            ISQLiteConnectionFactory factoryacc = new MvxDroidSQLiteConnectionFactory();

            var sqlitediracc = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filenameactionacc = sqlitediracc.Path + "/mysqlitesas.db";

            connacc = factoryacc.Create(filenameactionacc);
            connacc.CreateTable<Azurecon>();
            foreach (var e in connacc.Table<Azurecon>().Where(e => e.Sastring == "using"))
            {
                usercontainer = e.UserId;
            }
            connacc.Close();
            usercontainer = usercontainer.Replace("@", "");
            usercontainer = usercontainer.Replace(".", "");
            string azureurltoken = "http://93.118.34.239:8888/gettoken/" + usercontainer;
            var clienttk = new RestClient(azureurltoken);

            var requesttk = new RestRequest();
            IRestResponse responsetk = clienttk.Execute(requesttk);

            var myjsonstrtk = responsetk.Content;
            

            //End Get SAS connection string
            var client = new RestClient("http://93.115.97.151/azureservice/list.php?action=list&containerid=" + usercontainer);
        
            var request = new RestRequest();
            IRestResponse response = client.Execute(request);

            var myjsonstr = response.Content;
            JArray bloburlarr = JArray.Parse(myjsonstr);
            var countidx = 0;
            HashSet<string> myazureimagelist = new HashSet<string>();

            foreach (var perlurl in bloburlarr)
            {
                myazureimagelist.Add(perlurl["imgname"].ToString());
                //myazureimagenamelist.Add(perlurl["imgfilename"].ToString());
                /*
                var newanialele = new Animal() { Name = "Test Azure", Description = perlurl["imgname"].ToString(), Image = "Test Path", Mycheckbox = countidx };
                countidx++;
                myanimallist.Add(newanialele);
                 * */
            }
            
            foreach (var perlurlh in myazureimagelist)
            {
                string mymainazureurl = auzremainurl + usercontainer + "/" + perlurlh + "?" + myjsonstrtk;
                //Toast.MakeText(Application.Context, "Connect SAS String:" + mymainazureurl, ToastLength.Short).Show();   
                var newanialele = new Animal() { Name = "Test Azure", Description = mymainazureurl, Image = "Test Path", Mycheckbox = countidx };
                countidx++;
                myanimallist.Add(newanialele);

            }


            this.items = myanimallist;

        }
              
        public class MyAzureimage
        {
            public string imgname
            {
                get;
                set;
            }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //Get our object for this position
            var item = items[position];
            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // This gives us some performance gains by not always inflating a new view
            // This will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
            var view = (convertView ??
              context.LayoutInflater.Inflate(
                Resource.Layout.AzureImageList,
                parent,
                false)) as LinearLayout;
            //Find references to each subview in the list item's view
            var imageItem = view.FindViewById(Resource.Id.azureimageItem) as ImageView;
            var textTop = view.FindViewById(Resource.Id.azuretextTop) as TextView;
            var textBottom = view.FindViewById(Resource.Id.azuretextBottom) as TextView;
            //var mycheckbox = view.FindViewById(Resource.Id.azureCheckboxItem) as CheckBox;
            var viewazureimagebutton = view.FindViewById(Resource.Id.viewazureimagebutton) as Button;
            var deleteazureimagebutton = view.FindViewById(Resource.Id.deleteazureimagebutton) as Button;
            deleteazureimagebutton.Click += async delegate
            {
                /*
                var myazureimagenamestrtmp = item.Description;
                var myazureimagenamearr = myazureimagenamestrtmp.IndexOf(".jpg");
                var cutimgstr = myazureimagenamestrtmp.Substring(0, myazureimagenamearr);
                var imgstrarr = cutimgstr.Split(new Char[] { '/' }).ToList();
                var countidximgname = imgstrarr.Count - 1;
                var myrealimgname = imgstrarr[countidximgname] + ".jpg";
                var deleteazureurl = "http://93.115.97.151/azureservice/list.php?containerid=115708452302383620142&action=delete&filename=" + myrealimgname;
                //var deleteazureurl = "http://93.118.34.239:8888/deleteblob/115708452302383620142/" + myrealimgname;
                var browser = new WebView(Application.Context);
                browser.LoadUrl(deleteazureurl);

                Toast.MakeText(Application.Context, "File has been delete", ToastLength.Long).Show();
                 * */
                
                try
                {
                    var myazureimageurldown = new Uri(item.Description);
                    var cloudBlob = new CloudBlockBlob(myazureimageurldown);

                    await cloudBlob.DeleteIfExistsAsync();
                    Toast.MakeText(Application.Context, "File has been delete", ToastLength.Long).Show();

                }
                catch { }
                

                /*
                    var myazureimagenamestrtmp = item.Description;
                    var myazureimagenamearr = myazureimagenamestrtmp.IndexOf(".jpg");
                    var cutimgstr = myazureimagenamestrtmp.Substring(0, myazureimagenamearr);
                    var imgstrarr = cutimgstr.Split(new Char[] { '/' }).ToList();
                    var countidximgname = imgstrarr.Count - 1;
                    var myrealimgname = imgstrarr[countidximgname] + ".jpg";
                    var deleteazureurl = "http://93.115.97.151/azureservice/list.php?containerid=115708452302383620142&action=delete&filename=" + myrealimgname;
                    Uri deleteazureuri = new Uri(deleteazureurl);
                    var client = new RestClient(deleteazureurl);
                    var request = new RestRequest();
                    client.ExecuteAsync(request, response =>
                    {
                        //Console.WriteLine(response.Content);
                        Toast.MakeText(Application.Context, response.Content, ToastLength.Short).Show();
                    });
                    //IRestResponse response = client.Execute(request);

                    //var myjsonstr = response.Content;
                */

                
                

            };
            //save value to sqlite when checkbox click
            /*
            mycheckbox.Text = item.Description;
            mycheckbox.Click += (o, e) =>
            {
                if (mycheckbox.Checked)
                {

                    var mycbitemid = mycheckbox.Text;
                    Toast.MakeText(Application.Context, mycbitemid, ToastLength.Short).Show();

                    ISQLiteConnection connactionc = null;
                    ISQLiteConnectionFactory factoryc = new MvxDroidSQLiteConnectionFactory();

                    var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                    string filename = sqlitedir.Path + "/mysqliteaction.db";
                    //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();

                    connactionc = factoryc.Create(filename);
                    connactionc.CreateTable<MyCheckbox>();
                    connactionc.Insert(new MyCheckbox() { Name = mycbitemid });
                    connactionc.Close();

                    //var mycbitemid = mycheckbox.Text;
                    //myCollection.Add(mycbitemid);
                    //Toast.MakeText(Application.Context, myCollection.Count.ToString(), ToastLength.Short).Show();
                }

            };
             * */
            //End save value
            viewazureimagebutton.Click += async delegate
            {
                var myazureimageurldown = new Uri(item.Description);
                var cloudBlob = new CloudBlockBlob(myazureimageurldown);

                MemoryStream imageStream = new MemoryStream();
                var myazureimagenamestrtmp = item.Description;
                var myazureimagenamearr = myazureimagenamestrtmp.IndexOf(".jpg");
                var cutimgstr = myazureimagenamestrtmp.Substring(0, myazureimagenamearr);
                var imgstrarr = cutimgstr.Split(new Char[] { '/' }).ToList();
                var countidximgname = imgstrarr.Count - 1;
                var myrealimgname = imgstrarr[countidximgname] + ".jpg";
                await cloudBlob.DownloadToStreamAsync(imageStream);
                var myfiledir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "");
                string myfilename = myfiledir.Path + "/" + myrealimgname;
                

                /*
                using (FileStream file = new FileStream(myfilename, FileMode.Create, System.IO.FileAccess.Write))
                {
                    imageStream.CopyTo(file);
                    imageStream.Close();
                }
                 * */
                using (var fileStream = File.Create(myfilename))
                {
                    imageStream.Seek(0, SeekOrigin.Begin);
                    imageStream.CopyTo(fileStream);
                    Toast.MakeText(Application.Context, "Saved file into:" + myfilename, ToastLength.Short).Show();
                }
                
            };
            
            //Assign this item's values to the various subviews
            //Bitmap bitmap;
            //bitmap = BitmapFactory.DecodeFile(item.Image);

            // First we get the the dimensions of the file on disk

            //var myuri = getImageUri(item.Description);

            //Uri contentUri = Uri.FromFile(myimgfile);
            //Koush.UrlImageViewHelper.SetUrlDrawable(imageItem, item.Image);
            //Koush.UrlImageViewHelper.SetUrlDrawable(imageItem, "https://s.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=300");
            Koush.UrlImageViewHelper.SetUrlDrawable(imageItem, item.Description);
            Uri contentUri = new Uri(item.Description);

            
            textTop.SetText(item.Name, TextView.BufferType.Normal);
            //textBottom.SetText(item.Description, TextView.BufferType.Normal);
            textBottom.SetText(item.Description, TextView.BufferType.Normal);
            //textBottom.SetText("", TextView.BufferType.Normal);
            //Finally return the view
            return view;
        }

       
        private Android.Net.Uri getImageUri(String path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }


        public Animal GetItemAtPosition(int position)
        {
            return items[position];
        }


    }
}