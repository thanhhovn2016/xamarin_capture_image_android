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
using Uri = Android.Net.Uri;
using Android.Provider;
namespace CropImage
{

    public class CustomListAdapter : BaseAdapter
    {



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
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Date { get; set; }
            public string Imagepath { get; set; }
        }
        Activity context;
        public List<Animal> items;
        public List<MyCheckbox> mycbitems;
        public CustomListAdapter(Activity context) //We need a context to inflate our row view from
            : base()
        {
            this.context = context;
            //For demo purposes we hard code some data here
            //sqlite save
            var myanimallist = new List<Animal>();
            ISQLiteConnection conn = null;
            ISQLiteConnection connactiond = null;
            ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();
            ISQLiteConnectionFactory factoryd = new MvxDroidSQLiteConnectionFactory();
            
            
            

            var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filenameaction = sqlitedir.Path + "/mysqliteaction.db";

            connactiond = factoryd.Create(filenameaction);
            connactiond.CreateTable<MyCheckbox>();
            connactiond.CreateCommand("DELETE FROM MyCheckbox").ExecuteNonQuery();
            connactiond.Dispose();
            connactiond.Commit();
            connactiond.Close();
            if (File.Exists(filenameaction))
                File.Delete(filenameaction);
            string filename = sqlitedir.Path + "/mysqliteimage.db";
            //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();
            
            conn = factory.Create(filename);
            conn.CreateTable<Myimage>();



            var countidx = 0;
            foreach (var e in conn.Table<Myimage>().Where(e => e.Date == "30-12-2016"))
            {
                var mystrarray = e.Imagepath.Split('/');
                var myeleidx = mystrarray.Length -1;
                var newanialele = new Animal() { Name = mystrarray[myeleidx], Description = e.Imagepath, Image = e.Imagepath, Mycheckbox = countidx };
                myanimallist.Add(newanialele);
                countidx++;
            }
            //Toast.MakeText(this, mycount.ToString(), ToastLength.Short).Show();
            conn.Close();
            this.items = myanimallist;
            //sqlite save end            

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
                Resource.Layout.MyListRows,
                parent,
                false)) as LinearLayout;
            //Find references to each subview in the list item's view
            var imageItem = view.FindViewById(Resource.Id.imageItem) as ImageView;
            var textTop = view.FindViewById(Resource.Id.textTop) as TextView;
            var textBottom = view.FindViewById(Resource.Id.textBottom) as TextView;
            var mycheckbox = view.FindViewById(Resource.Id.CheckboxItem) as CheckBox;
            
            mycheckbox.Text = item.Name;
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
            //Assign this item's values to the various subviews
            //Bitmap bitmap;
            //bitmap = BitmapFactory.DecodeFile(item.Image);

            // First we get the the dimensions of the file on disk

            //var myuri = getImageUri(item.Description);
          
            //Uri contentUri = Uri.FromFile(myimgfile);
            Koush.UrlImageViewHelper.SetUrlDrawable(imageItem, item.Image);
            //Koush.UrlImageViewHelper.SetUrlDrawable(imageItem, "https://s.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=300");

            //imageItem.SetImageBitmap(resizedBitmap);
            
            //imageItem.SetImageBitmap(resizedBitmap);
            textTop.SetText(item.Name, TextView.BufferType.Normal);
            //textBottom.SetText(item.Description, TextView.BufferType.Normal);
            textBottom.SetText(item.Description, TextView.BufferType.Normal);
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