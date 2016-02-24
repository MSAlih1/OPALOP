using Instagram.api;
using Instagram.api.classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QPS_Web1._CSHARP.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace QPS_Web1._QPS.Class
{
    public static class InstagramProfile
    {
        private static string RequestGetToUrl(string url)
        {
            WebProxy proxy = WebProxy.GetDefaultProxy();
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.IndexOf("://") <= 0)
                url = "http://" + url.Replace(",", ".");

            try
            {
                using (var client = new WebClient())
                {
                    //proxy
                    if (proxy != null)
                        client.Proxy = proxy;

                    //response
                    byte[] response = client.DownloadData(url);
                    //out
                    var enc = new UTF8Encoding();
                    string outp = enc.GetString(response);
                    return outp;
                }
            }
            catch (WebException ex)
            {
                string err = ex.Message;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return null;
        }

        public static List<string> UserPhotos()
        {
            UserInformation info = getUser();
            string isInstagram = info.user_id.Split('|')[0];
            string userid = info.user_id.Split('|')[1];

            if (isInstagram == "instagram")
            {
                InstagramApiWrapper wrap = InstagramApiWrapper.GetInstance(new Configuration(getAccessToken(), userid));
                InstagramResponse<InstagramMedia[]> mediam = wrap.CurrentUserRecentMedia(33, "", "");
                List<InstagramMedia> photos = mediam.data.ToList();
                int Photocount = photos.Count;
                do
                {
                    mediam = wrap.CurrentUserRecentMedia(33, "", mediam.pagination.next_max_id);
                    InstagramMedia[] photos2 = mediam.data;
                    photos.AddRange(photos2);
                    Photocount += photos2.Length;
                    photos2 = null;
                }
                while (mediam.pagination.next_max_id != null);
                List<string> imgList = new List<string>();

                if (Photocount > 0)
                {
                    foreach (var item in photos)
                        imgList.Add(item.images.thumbnail.url);
                }
                mediam = null;
                info = null;
                return imgList;
            }
            return null;
        }

        public static List<QuardPixAvg> SaveUserPhoto(string UserName, string item)
        {
            qprPath resources = new qprPath(UserName);
            return new List<QuardPixAvg>();
        }

        public static Bitmap DownloadImage(string _imageUrl)
        {
            try
            {
                WebClient client = new WebClient();
                Bitmap bitmap = null;
                Stream stream = null;
                try
                {
                    stream = client.OpenRead(_imageUrl);
                    bitmap = new Bitmap(stream);
                }
                catch (Exception)
                {
                    return null;
                }
                stream.Flush();
                stream.Close();
                stream = null;
                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string getAuthorization()
        {
            return HttpContext.Current.Request.Headers["Authorization"].Replace("Bearer ", "");
        }

        public static string getAccessToken()
        {
            UserInformation inf = getUser();
            Identity ident = inf.identities.Find(p => p.user_id == inf.user_id.Split('|')[1]);
            return ident.access_token;
        }

        public static UserInformation getUser()
        {
            string id_token = getAuthorization();
            string json = RequestGetToUrl(string.Format("https://qpiro.auth0.com/tokeninfo?id_token={0}", id_token));
            try
            {
                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return JsonConvert.DeserializeObject<UserInformation>(json, serSettings);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public class InstaPhoto
        {
            [JsonProperty("usethis")]
            public bool UseThis { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class Counts
        {
            [JsonProperty("media")]
            public int media { get; set; }

            [JsonProperty("followed_by")]
            public int followed_by { get; set; }

            [JsonProperty("follows")]
            public int follows { get; set; }
        }

        public class Identity
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }

            [JsonProperty("provider")]
            public string provider { get; set; }

            [JsonProperty("user_id")]
            public string user_id { get; set; }

            [JsonProperty("connection")]
            public string connection { get; set; }

            [JsonProperty("issocial")]
            public bool isSocial { get; set; }
        }

        public class UserInformation
        {
            [JsonProperty("picture")]
            public string picture { get; set; }

            [JsonProperty("nickname")]
            public string nickname { get; set; }

            [JsonProperty("bio")]
            public string bio { get; set; }

            [JsonProperty("website")]
            public string website { get; set; }

            [JsonProperty("full_name")]
            public string full_name { get; set; }

            [JsonProperty("counts")]
            public Counts counts { get; set; }

            [JsonProperty("clientid")]
            public string clientID { get; set; }

            [JsonProperty("user_id")]
            public string user_id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("identities")]
            public List<Identity> identities { get; set; }

            [JsonProperty("created_at")]
            public string created_at { get; set; }

            [JsonProperty("global_client_id")]
            public string global_client_id { get; set; }
        }
    }
}