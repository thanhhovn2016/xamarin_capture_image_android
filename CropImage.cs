/*
 * Copyright (C) 2009 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid;
using Java.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace CropImage
{
    /// <summary>
    /// The activity can crop specific region of interest from an image.
    /// </summary>
    [Activity]
    public class CropImage : MonitoredActivity
    {
        #region Private members

        // These are various options can be specified in the intent.
        private Bitmap.CompressFormat outputFormat = Bitmap.CompressFormat.Jpeg;
        private Android.Net.Uri saveUri = null;
        private int aspectX, aspectY;
        private Handler mHandler = new Handler();

        // These options specifiy the output image size and whether we should
        // scale the output to fit it (or just crop it).
        private int outputX, outputY;
        private bool scale;
        private bool scaleUp = true;

        private CropImageView imageView;
        private Bitmap bitmap;

        private string imagePath;

        private const int NO_STORAGE_ERROR = -1;
        private const int CANNOT_STAT_ERROR = -2;

        #endregion

        #region Properties

        public HighlightView Crop
        {
            set;
            get;
        }

        /// <summary>
        /// Whether the "save" button is already clicked.
        /// </summary>
        public bool Saving { get; set; }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.cropimage);

            imageView = FindViewById<CropImageView>(Resource.Id.image);

            showStorageToast(this);

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                imagePath = extras.GetString("image-path");

                saveUri = getImageUri(imagePath);
                if (extras.GetString(MediaStore.ExtraOutput) != null)
                {
                    saveUri = getImageUri(extras.GetString(MediaStore.ExtraOutput));
                }

                bitmap = getBitmap(imagePath);

                aspectX = extras.GetInt("aspectX");
                aspectY = extras.GetInt("aspectY");
                outputX = extras.GetInt("outputX");
                outputY = extras.GetInt("outputY");
                scale = extras.GetBoolean("scale", true);
                scaleUp = extras.GetBoolean("scaleUpIfNeeded", true);

                if (extras.GetString("outputFormat") != null)
                {
                    outputFormat = Bitmap.CompressFormat.ValueOf(extras.GetString("outputFormat"));
                }
            }

            if (bitmap == null)
            {
                Finish();
                return;
            }

            Window.AddFlags(WindowManagerFlags.Fullscreen);


            FindViewById<Button>(Resource.Id.discard).Click += (sender, e) => { SetResult(Result.Canceled); Finish(); };
            //FindViewById<Button>(Resource.Id.save).Click += async delegate { onSaveClicked(); };

            FindViewById<Button>(Resource.Id.save).Click += async delegate {

                if (Saving)
                {
                    return;
                }

                Saving = true;

                var r = Crop.CropRect;

                int width = r.Width();
                int height = r.Height();

                Bitmap croppedImage = Bitmap.CreateBitmap(width, height, Bitmap.Config.Rgb565);
                {
                    Canvas canvas = new Canvas(croppedImage);
                    Rect dstRect = new Rect(0, 0, width, height);
                    canvas.DrawBitmap(bitmap, r, dstRect, null);
                }

                // If the output is required to a specific size then scale or fill
                if (outputX != 0 && outputY != 0)
                {
                    if (scale)
                    {
                        // Scale the image to the required dimensions
                        Bitmap old = croppedImage;
                        croppedImage = Util.transform(new Matrix(),
                                                      croppedImage, outputX, outputY, scaleUp);
                        if (old != croppedImage)
                        {
                            old.Recycle();
                        }
                    }
                    else
                    {
                        // Don't scale the image crop it to the size requested.
                        // Create an new image with the cropped image in the center and
                        // the extra space filled.              
                        Bitmap b = Bitmap.CreateBitmap(outputX, outputY,
                                                       Bitmap.Config.Rgb565);
                        Canvas canvas = new Canvas(b);

                        Rect srcRect = Crop.CropRect;
                        Rect dstRect = new Rect(0, 0, outputX, outputY);

                        int dx = (srcRect.Width() - dstRect.Width()) / 2;
                        int dy = (srcRect.Height() - dstRect.Height()) / 2;

                        // If the srcRect is too big, use the center part of it.
                        srcRect.Inset(Math.Max(0, dx), Math.Max(0, dy));

                        // If the dstRect is too big, use the center part of it.
                        dstRect.Inset(Math.Max(0, -dx), Math.Max(0, -dy));

                        // Draw the cropped bitmap in the center
                        canvas.DrawBitmap(bitmap, srcRect, dstRect, null);

                        // Set the cropped bitmap as the new bitmap
                        croppedImage.Recycle();
                        croppedImage = b;
                    }
                }

                // Return the cropped image directly or save it to the specified URI.
                Bundle myExtras = Intent.Extras;

                if (myExtras != null &&
                    (myExtras.GetParcelable("data") != null || myExtras.GetBoolean("return-data")))
                {
                    Bundle extrasas = new Bundle();
                    extras.PutParcelable("data", croppedImage);
                    SetResult(Result.Ok,
                              (new Intent()).SetAction("inline-data").PutExtras(extrasas));
                    Finish();
                }
                else
                {
                    //Toast.MakeText(Application.Context, saveUri.ToString(), ToastLength.Long).Show();
                    //Toast.MakeText(Application.Context, "Upload Complete", ToastLength.Long).Show();

                    //Upload to Azure
                    ISQLiteConnection connacc = null;

                    ISQLiteConnectionFactory factoryacc = new MvxDroidSQLiteConnectionFactory();

                    var sqlitediracc = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                    string filenameactionacc = sqlitediracc.Path + "/mysqlitesas.db";

                    connacc = factoryacc.Create(filenameactionacc);
                    connacc.CreateTable<Azurecon>();
                    var useridconnc = "";
                    foreach (var eu in connacc.Table<Azurecon>().Where(eu => eu.Sastring == "using"))
                    {
                        useridconnc = eu.UserId;
                    }
                    connacc.Close();
                    //myuserid = "115708452302383620142";
                    useridconnc = useridconnc.Replace("@","");
                    useridconnc = useridconnc.Replace(".", "");
                    var myurl = "http://93.118.34.239:8888/" + useridconnc;
                    Uri azureuri = new Uri(myurl);


                    HttpWebRequest request = new HttpWebRequest(azureuri);
                    request.Method = "GET";


                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    try
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {

                            string responseString = sr.ReadToEnd();
                            Toast.MakeText(this, saveUri.ToString(), ToastLength.Short).Show();
                            try
                            {
                                await UseContainerSAS(responseString, saveUri.ToString());
                            }
                            catch
                            {
                            }


                        }
                    }
                    catch { 
                    }
                    //End Upload to Azure
                    
                    Bitmap b = croppedImage;
                    BackgroundJob.StartBackgroundJob(this, null, "Saving image", () => saveOutput(b), mHandler);
                }            
            
            };

            FindViewById<Button>(Resource.Id.rotateLeft).Click += (o, e) =>
            {
                bitmap = Util.rotateImage(bitmap, -90);
                RotateBitmap rotateBitmap = new RotateBitmap(bitmap);
                imageView.SetImageRotateBitmapResetBase(rotateBitmap, true);
                addHighlightView();
            };

            FindViewById<Button>(Resource.Id.rotateRight).Click += (o, e) =>
            {
                bitmap = Util.rotateImage(bitmap, 90);
                RotateBitmap rotateBitmap = new RotateBitmap(bitmap);
                imageView.SetImageRotateBitmapResetBase(rotateBitmap, true);
                addHighlightView();
            };

            imageView.SetImageBitmapResetBase(bitmap, true);
            addHighlightView();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (bitmap != null && bitmap.IsRecycled)
            {
                bitmap.Recycle();
            }
        }

        #endregion

        #region Private helpers

        private void addHighlightView()
        {
            Crop = new HighlightView(imageView);

            int width = bitmap.Width;
            int height = bitmap.Height;

            Rect imageRect = new Rect(0, 0, width, height);

            // make the default size about 4/5 of the width or height
            int cropWidth = Math.Min(width, height) * 4 / 5;
            int cropHeight = cropWidth;

            if (aspectX != 0 && aspectY != 0)
            {
                if (aspectX > aspectY)
                {
                    cropHeight = cropWidth * aspectY / aspectX;
                }
                else
                {
                    cropWidth = cropHeight * aspectX / aspectY;
                }
            }

            int x = (width - cropWidth) / 2;
            int y = (height - cropHeight) / 2;

            RectF cropRect = new RectF(x, y, x + cropWidth, y + cropHeight);
            Crop.Setup(imageView.ImageMatrix, imageRect, cropRect, aspectX != 0 && aspectY != 0);

            imageView.ClearHighlightViews();
            Crop.Focused = true;
            imageView.AddHighlightView(Crop);
        }

        private Android.Net.Uri getImageUri(String path)
        {
            return Android.Net.Uri.FromFile(new Java.IO.File(path));
        }

        private Bitmap getBitmap(String path)
        {
            var uri = getImageUri(path);
            System.IO.Stream ins = null;

            try
            {
                int IMAGE_MAX_SIZE = 1024;
                ins = ContentResolver.OpenInputStream(uri);

                // Decode image size
                BitmapFactory.Options o = new BitmapFactory.Options();
                o.InJustDecodeBounds = true;

                BitmapFactory.DecodeStream(ins, null, o);
                ins.Close();

                int scale = 1;
                if (o.OutHeight > IMAGE_MAX_SIZE || o.OutWidth > IMAGE_MAX_SIZE)
                {
                    scale = (int)Math.Pow(2, (int)Math.Round(Math.Log(IMAGE_MAX_SIZE / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
                }

                BitmapFactory.Options o2 = new BitmapFactory.Options();
                o2.InSampleSize = scale;
                ins = ContentResolver.OpenInputStream(uri);
                Bitmap b = BitmapFactory.DecodeStream(ins, null, o2);
                ins.Close();

                return b;
            }
            catch (Exception e)
            {
                Log.Error(GetType().Name, e.Message);
            }

            return null;
        }

        private void onSaveClicked()
        {
            // TODO this code needs to change to use the decode/crop/encode single
            // step api so that we don't require that the whole (possibly large)
            // bitmap doesn't have to be read into memory

        }

        //Async upload to Azure
        static async Task UseContainerSAS(string sas,string fileonsdcard)
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
                        string imgfilename = fileonsdcard;
                        var myazureimagenamestrtmp = imgfilename;
                        var myazureimagenamearr = myazureimagenamestrtmp.IndexOf(".jpg");
                        var cutimgstr = myazureimagenamestrtmp.Substring(0, myazureimagenamearr);
                        var imgstrarr = cutimgstr.Split(new Char[] { '/' });
                        var countidximgnamett = 0;
                        foreach (var perele in imgstrarr)
                        {
                            countidximgnamett++;
                        }
                        var countidximgname = countidximgnamett - 1;
                        var myrealimgname = imgstrarr[countidximgname] + ".jpg";

                        //Toast.MakeText(Application.Context, myrealimgname, ToastLength.Short).Show();
                        var imgdir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                        imgfilename = imgdir + "/" + myrealimgname;
                        //Toast.MakeText(Application.Context, myrealimgname, ToastLength.Short).Show();
                        using (FileStream fileStream = new FileStream(imgfilename, FileMode.Open, FileAccess.Read))
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            CloudBlockBlob blob = container.GetBlockBlobReference(myrealimgname);
                            fileStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            Toast.MakeText(Application.Context, sas, ToastLength.Short).Show();
                            using (memoryStream)
                            {
                                await blob.UploadFromStreamAsync(memoryStream);
                                Toast toast = Toast.MakeText(Application.Context, "Upload Complete", ToastLength.Short);
                                toast.Show();
                            }
                        }
                }

                catch (Java.Lang.Exception e)
                {

                }
            }
            catch (Java.Lang.Exception eerror)
            { }

        }
        //End Async
        public class Myimage
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Date { get; set; }
            public string Imagepath { get; set; }
        }
        private void saveOutput(Bitmap croppedImage)
        {
            if (saveUri != null)
            {
                try
                {
                    using (var outputStream = ContentResolver.OpenOutputStream(saveUri))
                    {
                        if (outputStream != null)
                        {
                            croppedImage.Compress(outputFormat, 75, outputStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(this.GetType().Name, ex.Message);
                }

                Bundle extras = new Bundle();
                SetResult(Result.Ok, new Intent(saveUri.ToString())
                          .PutExtras(extras));
                
            }
            else
            {
                Log.Error(this.GetType().Name, "not defined image url");
            }
            //sqlite save

            ISQLiteConnection conn = null;
            ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

            var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            string filename = sqlitedir.Path + "/mysqliteimage.db";
            //Toast.MakeText(Application.Context, filename, ToastLength.Long).Show();
            Java.IO.File f = new Java.IO.File(filename);
            conn = factory.Create(filename);
            conn.CreateTable<Myimage>();
            
            
            conn.Insert(new Myimage() { Date = "30-12-2016", Imagepath = saveUri.ToString() });
            var mycount = 0;
            foreach (var e in conn.Table<Myimage>().Where(e => e.Date == "30-12-2016"))
            {
                mycount++;
            }
            //Toast.MakeText(this, mycount.ToString(), ToastLength.Short).Show();
            conn.Close();
            //sqlite save end            
            croppedImage.Recycle();
            Finish();
        }

        private static void showStorageToast(Activity activity)
        {
            showStorageToast(activity, calculatePicturesRemaining());
        }

        private static void showStorageToast(Activity activity, int remaining)
        {
            string noStorageText = null;

            if (remaining == NO_STORAGE_ERROR)
            {
                String state = Android.OS.Environment.ExternalStorageState;
                if (state == Android.OS.Environment.MediaChecking)
                {
                    noStorageText = "Preparing card";
                }
                else
                {
                    noStorageText = "No storage card";
                }
            }
            else if (remaining < 1)
            {
                noStorageText = "Not enough space";
            }

            if (noStorageText != null)
            {
                Toast.MakeText(activity, noStorageText, ToastLength.Long).Show();
            }
        }

        private static int calculatePicturesRemaining()
        {
            try
            {
                string storageDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures).ToString();
                StatFs stat = new StatFs(storageDirectory);
                float remaining = ((float)stat.AvailableBlocks
                                   * (float)stat.BlockSize) / 400000F;
                return (int)remaining;
            }
            catch (Exception)
            {
                // if we can't stat the filesystem then we don't know how many
                // pictures are remaining.  it might be zero but just leave it
                // blank since we really don't know.
                return CANNOT_STAT_ERROR;
            }
        }

        #endregion
    }
}
