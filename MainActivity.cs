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
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.Lang;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Cirrious.MvvmCross.Community.Plugins.Sqlite.Droid;
using Android.Webkit;
namespace CropImage
{
    [Activity(Label = "CropImage", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private const int PICK_FROM_CAMERA = 1;
        private Android.Net.Uri mImageCaptureUri;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            /*
            //Facebook login api
            var auth = new OAuth2Authenticator(
                clientId: "1270606036289960",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
//                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));
                redirectUrl: new Uri("http://myjobupwork.com/fblogin/"));
             
            
            //StartActivity (auth.GetUI(Application.Context));
            
            auth.Completed += (sender, eventArgs) =>
            {
                // We presented the UI, so it's up to us to dimiss it on iOS.


                if (eventArgs.IsAuthenticated)
                {
                    // Use eventArgs.Account to do wonderful things
                    
                    string access_token;
                    eventArgs.Account.Properties.TryGetValue("access_token", out access_token);
                    //Toast.MakeText(this, "Authenticate Token:" + access_token, ToastLength.Short).Show();
                    var myurl = "https://graph.facebook.com/me?access_token=" + access_token;
                    Uri uri = new Uri(myurl);
                    HttpWebRequest request = new HttpWebRequest(uri);
                    request.Method = "GET";


                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = sr.ReadToEnd();
                        Newtonsoft.Json.Linq.JObject myjObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        var myid = (string)myjObject["id"];
                        Toast.MakeText(this, "Your Facebook UserId:" + myid, ToastLength.Short).Show();

                    }
                    response.Close();
                }
            };
            */  
            /*
            //Google Login 
            var auth = new OAuth2Authenticator(clientId: "544771199531-lfe6dn212h2ch38f5i4uaah5j7c2qs00.apps.googleusercontent.com",
scope: "https://www.googleapis.com/auth/userinfo.email",
authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
redirectUrl: new Uri("http://myjobupwork.com/ggplus/"),
getUsernameAsync: null);
            

            auth.Completed += async (sender, e) =>
            {
                if (!e.IsAuthenticated)
                {
                    Toast.MakeText(this, "Fail to authenticate!", ToastLength.Short).Show();
                    return;
                }
                string access_token;
                e.Account.Properties.TryGetValue("access_token", out access_token);
               
                var myurl = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + access_token;
                Uri uri = new Uri(myurl);
                HttpWebRequest request = new HttpWebRequest(uri);
                request.Method = "GET";


                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string responseString = sr.ReadToEnd();
                    Newtonsoft.Json.Linq.JObject myjObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                    var myid = (string)myjObject["id"];
                    ISQLiteConnection conn = null;

                    ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

                    var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                    string filenameaction = sqlitedir.Path + "/mysqlitesas.db";

                    conn = factory.Create(filenameaction);
                    conn.CreateTable<Azurecon>();
                    conn.DeleteAll<Azurecon>();
                    conn.Insert(new Azurecon() { Sastring = "", UserType = "Google", UserId = myid });
                    Toast.MakeText(this, "Your Google UserId:" + myid, ToastLength.Short).Show();
                    conn.Close();
                }
                response.Close();
            };
            
            var intent = auth.GetUI(this);
            StartActivity(intent);
             
            // Set our view from the "main" layout resource
            */
            //login layout xml
            SetContentView(Resource.Layout.Main);
            Button buttonfacelogin = FindViewById<Button>(Resource.Id.buttonfacebooklogin);
            Button buttongooglelogin = FindViewById<Button>(Resource.Id.buttongooglelogin);
            buttonfacelogin.Click += delegate
            {
                //Facebook login api
                var auth = new OAuth2Authenticator(
                    clientId: "1270606036289960",
                    scope: "email",
                    authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                    //                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));
                    redirectUrl: new Uri("http://myjobupwork.com/fblogin/"));


                //StartActivity (auth.GetUI(Application.Context));

                auth.Completed += (sender, eventArgs) =>
                {
                    // We presented the UI, so it's up to us to dimiss it on iOS.
                    if (!eventArgs.IsAuthenticated)
                    {
                        Toast.MakeText(this, "Fail to authenticate!", ToastLength.Short).Show();
                        return;
                    }

                    if (eventArgs.IsAuthenticated)
                    {
                        // Use eventArgs.Account to do wonderful things

                        string access_token;
                        eventArgs.Account.Properties.TryGetValue("access_token", out access_token);
                        //Toast.MakeText(this, "Authenticate Token:" + access_token, ToastLength.Short).Show();
                        var myurl = "https://graph.facebook.com/me?fields=id,email&access_token=" + access_token;
                        
                        Uri uri = new Uri(myurl);
                        HttpWebRequest request = new HttpWebRequest(uri);
                        request.Method = "GET";


                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            string responseString = sr.ReadToEnd();
                            Newtonsoft.Json.Linq.JObject myjObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                            var myid = (string)myjObject["id"];
                            var myemail = (string)myjObject["email"];
                            var myurlui = "https://graph.facebook.com/me/permissions" + "?fields=id,email" + "&access_token=" + access_token;
                        
                        
                                                   
                            ISQLiteConnection conn = null;

                            ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

                            var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                            string filenameaction = sqlitedir.Path + "/mysqlitesas.db";
                            string myuseridfb = "fb" + myid;
                            if (myemail.Length < 3)
                            {
                                myemail = myuseridfb;
                            }

                            conn = factory.Create(filenameaction);
                            conn.CreateTable<Azurecon>();
                            //conn.CreateCommand("Delete from Azurecon").ExecuteNonQuery();
                            int countrow = 0;
                            foreach (var eu in conn.Table<Azurecon>().Where(eu => eu.UserType == "Facebook"))
                            {
                                countrow++;
                            }
                            if (countrow > 0)
                            {
                                conn.CreateCommand("Update Azurecon set Sastring = '' where UserType='Google'").ExecuteNonQuery();
                                conn.CreateCommand("Update Azurecon set UserId = '" + myemail + "' where UserType='Facebook'").ExecuteNonQuery();
                            }
                            else
                            {
                                conn.Insert(new Azurecon() { Sastring = "", UserType = "Facebook", UserId = myemail });
                            }

                            //conn.Insert(new Azurecon() { Sastring = "", UserType = "Facebook", UserId = myemail });
                            Toast.MakeText(this, "Your Facebook UserId:" + myemail, ToastLength.Short).Show();
                            //Create new container for Google userid
                            var myemailtrim = myemail.Replace("@", "");
                            myemailtrim = myemailtrim.Replace(".","");
                            string myauzreafaceusercon = "http://93.118.34.239:8888/createcon/" + myemailtrim;
                            var browser = new WebView(Application.Context);
                            browser.LoadUrl(myauzreafaceusercon);

                            conn.Close();
                        }
                        response.Close();
                        SetContentView(Resource.Layout.Main1);
                        Button button = FindViewById<Button>(Resource.Id.button);
                        Button showlistbutton = FindViewById<Button>(Resource.Id.mylist);
                        //Button uploadtoazure = FindViewById<Button>(Resource.Id.);
                        button.Click += (s, e) => doTakePhotoAction();
                        showlistbutton.Click += delegate
                        {
                            StartActivity(typeof(MyListviewActivity));
                        };
                        Button showauzreimagebutton = FindViewById<Button>(Resource.Id.azureimagelist);
                        showauzreimagebutton.Click += delegate
                        {
                            StartActivity(typeof(AzureActivity));
                        };

                    }
                };
                ISQLiteConnection connch = null;

                ISQLiteConnectionFactory factorych = new MvxDroidSQLiteConnectionFactory();

                var sqlitedirch = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                string filenameactionch = sqlitedirch.Path + "/mysqlitesas.db";
                connch = factorych.Create(filenameactionch);
                connch.CreateTable<Azurecon>();
                //conn.CreateCommand("Delete from Azurecon").ExecuteNonQuery();
                int countrowch = 0;
                foreach (var euch in connch.Table<Azurecon>().Where(euch => euch.UserType == "Facebook"))
                {
                    countrowch++;
                }
                if (countrowch > 0)
                {
                    connch.CreateCommand("Update Azurecon set Sastring = '' where UserType='Google'").ExecuteNonQuery();
                    connch.CreateCommand("Update Azurecon set Sastring = 'using' where UserType='Facebook'").ExecuteNonQuery();
                    SetContentView(Resource.Layout.Main1);
                    Button button = FindViewById<Button>(Resource.Id.button);
                    Button showlistbutton = FindViewById<Button>(Resource.Id.mylist);
                    //Button uploadtoazure = FindViewById<Button>(Resource.Id.);
                    button.Click += (s, e) => doTakePhotoAction();
                    showlistbutton.Click += delegate
                    {
                        StartActivity(typeof(MyListviewActivity));
                    };
                    Button showauzreimagebutton = FindViewById<Button>(Resource.Id.azureimagelist);
                    showauzreimagebutton.Click += delegate
                    {
                        StartActivity(typeof(AzureActivity));
                    };

                }
                else
                {
                    var intent = auth.GetUI(this);
                    StartActivity(intent);                    
                }

            
            };
            buttongooglelogin.Click += delegate
            {
                //Google Login 
                var auth = new OAuth2Authenticator(clientId: "544771199531-lfe6dn212h2ch38f5i4uaah5j7c2qs00.apps.googleusercontent.com",
    scope: "https://www.googleapis.com/auth/userinfo.email",
    authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
    redirectUrl: new Uri("http://myjobupwork.com/ggplus/"),
    getUsernameAsync: null);


                auth.Completed += async (sender, e) =>
                {
                    if (!e.IsAuthenticated)
                    {
                        Toast.MakeText(this, "Fail to authenticate!", ToastLength.Short).Show();
                        return;
                        //SetContentView(Resource.Layout.Main);
                    }else{
                    string access_token;
                    e.Account.Properties.TryGetValue("access_token", out access_token);

                    var myurl = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + access_token;
                    Uri uri = new Uri(myurl);
                    HttpWebRequest request = new HttpWebRequest(uri);
                    request.Method = "GET";


                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = sr.ReadToEnd();
                        Newtonsoft.Json.Linq.JObject myjObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                        var myid = (string)myjObject["id"];
                        var myemail = (string)myjObject["email"];
                        ISQLiteConnection conn = null;

                        ISQLiteConnectionFactory factory = new MvxDroidSQLiteConnectionFactory();

                        var sqlitedir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                        string filenameaction = sqlitedir.Path + "/mysqlitesas.db";
                        conn = factory.Create(filenameaction);
                        conn.CreateTable<Azurecon>();
                        int countrow = 0;
                        foreach (var eu in conn.Table<Azurecon>().Where(eu => eu.UserType == "Google"))
                        {
                            countrow++;
                        }
                        if (countrow > 0)
                        {
                            conn.CreateCommand("Update Azurecon set Sastring = '' where UserType='Facebook'").ExecuteNonQuery();
                            conn.CreateCommand("Update Azurecon set Sastring = 'using',UserId = '" + myemail + "' where UserType='Google'").ExecuteNonQuery();
                        }
                        else {
                            conn.Insert(new Azurecon() { Sastring = "", UserType = "Google", UserId = myemail });
                        }

                        //get googleuser email

                        Toast.MakeText(this, "Your Google UserId:" + myemail, ToastLength.Short).Show();
                        //Create new container for Google userid
                        var myemailtrim = myemail.Replace("@", "");
                        myemailtrim = myemailtrim.Replace(".", "");
                        string myauzregoogleusercon = "http://93.118.34.239:8888/createcon/" + myemailtrim;
                        var browser = new WebView(Application.Context);
                        browser.LoadUrl(myauzregoogleusercon);

                        conn.Close();
                    }
                    response.Close();

                    SetContentView(Resource.Layout.Main1);
                    Button button = FindViewById<Button>(Resource.Id.button);
                    Button showlistbutton = FindViewById<Button>(Resource.Id.mylist);
                    //Button uploadtoazure = FindViewById<Button>(Resource.Id.);
                    button.Click += (s, ea) => doTakePhotoAction();
                    showlistbutton.Click += delegate
                    {
                        StartActivity(typeof(MyListviewActivity));
                    };
                    Button showauzreimagebutton = FindViewById<Button>(Resource.Id.azureimagelist);
                    showauzreimagebutton.Click += delegate
                    {
                        StartActivity(typeof(AzureActivity));
                    };
                }
                };

                ISQLiteConnection connch = null;

                ISQLiteConnectionFactory factorych = new MvxDroidSQLiteConnectionFactory();

                var sqlitedirch = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
                string filenameactionch = sqlitedirch.Path + "/mysqlitesas.db";
                connch = factorych.Create(filenameactionch);
                connch.CreateTable<Azurecon>();
                //conn.CreateCommand("Delete from Azurecon").ExecuteNonQuery();
                int countrowch = 0;
                foreach (var euch in connch.Table<Azurecon>().Where(euch => euch.UserType == "Google"))
                {
                    countrowch++;
                }
                if (countrowch > 0)
                {
                    connch.CreateCommand("Update Azurecon set Sastring = '' where UserType='Facebook'").ExecuteNonQuery();
                    connch.CreateCommand("Update Azurecon set Sastring = 'using' where UserType='Google'").ExecuteNonQuery();
                    SetContentView(Resource.Layout.Main1);
                    Button button = FindViewById<Button>(Resource.Id.button);
                    Button showlistbutton = FindViewById<Button>(Resource.Id.mylist);
                    //Button uploadtoazure = FindViewById<Button>(Resource.Id.);
                    button.Click += (s, e) => doTakePhotoAction();
                    showlistbutton.Click += delegate
                    {
                        StartActivity(typeof(MyListviewActivity));
                    };
                    Button showauzreimagebutton = FindViewById<Button>(Resource.Id.azureimagelist);
                    showauzreimagebutton.Click += delegate
                    {
                        StartActivity(typeof(AzureActivity));
                    };

                }
                else
                {
                    var intent = auth.GetUI(this);
                    StartActivity(intent);
                }
            };
            // Get our button from the layout resource,
            // and attach an event to it
            /*
            SetContentView(Resource.Layout.Main1);
            Button buttonmain = FindViewById<Button>(Resource.Id.button);
            Button showlistbuttonmain = FindViewById<Button>(Resource.Id.mylist);
            //Button uploadtoazure = FindViewById<Button>(Resource.Id.);
            buttonmain.Click += (s, e) => doTakePhotoAction();
            showlistbuttonmain.Click += delegate
            {
                StartActivity(typeof(MyListviewActivity));
            };
            Button showauzreimagebuttonmain = FindViewById<Button>(Resource.Id.azureimagelist);
            showauzreimagebuttonmain.Click += delegate
            {
                StartActivity(typeof(AzureActivity));
            };
             * */
            
        }
            

        private Java.IO.File createDirectoryForPictures()
        {
            var dir = new Java.IO.File(global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures), "Boruto");
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            return dir;
        }

        private void doTakePhotoAction()
        {

            Intent intent = new Intent(MediaStore.ActionImageCapture);

            mImageCaptureUri = Android.Net.Uri.FromFile(new Java.IO.File(createDirectoryForPictures(), string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid())));

            intent.PutExtra(MediaStore.ExtraOutput, mImageCaptureUri);

            try
            {
                intent.PutExtra("return-data", false);
                StartActivityForResult(intent, PICK_FROM_CAMERA);
            }
            catch (ActivityNotFoundException e)
            {
                e.PrintStackTrace();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            if (resultCode != Result.Ok)
            {
                return;
            }

            switch (requestCode)
            {
                case PICK_FROM_CAMERA:
                    Intent intent = new Intent(this, typeof(CropImage));
                    intent.PutExtra("image-path", mImageCaptureUri.Path);
                    intent.PutExtra("scale", true);
                    StartActivity(intent);
                    break;
            }
        }

 
    }
}


