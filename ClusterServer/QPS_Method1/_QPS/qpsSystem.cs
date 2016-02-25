using ImageMagick;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QPS_Method1._QPS;
using QPS_Web1._CSHARP.Class;
using QPS_Web1._QPS.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace QPS_Web1._QPS.Class
{
    public static class qpsSystem
    {
        public static string GetAccessCode()
        { return "14E0057D-E497-47CE-BB0B-0D3096AC5D87"; }

        public static void UpdateSelectedInstaPhotos(string UserName, string ls)
        {
            qprPath resources = new qprPath(UserName);
            try
            {
                string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);

                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                InstagramProfile.InstaPhoto[] outObject = JsonConvert.DeserializeObject<InstagramProfile.InstaPhoto[]>(ls, serSettings);

                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri");

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + UserName).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                IEnumerable<XElement> photos = InstagramP.Elements("Photos");

                if (outObject.Count() > 0)
                {
                    foreach (XElement photo in photos)
                    {
                        foreach (InstagramProfile.InstaPhoto selectedPhoto in outObject)
                        {
                            if (photo.Value == selectedPhoto.name)
                            {
                                XAttribute useths = photo.Attribute("useThis");
                                useths.Value = selectedPhoto.UseThis.ToString();
                                break;
                            }
                        }
                    }
                    doc.Save(file[0]);
                }
            }
            catch (Exception ef)
            {

                throw ef;
            }
        }

        public static void CreateDir(string UserName)
        {
            try
            {
                qprPath resources = new qprPath(UserName);

                if (!Directory.Exists(resources.ResourcePhotos_Path))
                    Directory.CreateDirectory(resources.ResourcePhotos_Path);

                if (!Directory.Exists(resources.SavedPhotos_Path))
                    Directory.CreateDirectory(resources.SavedPhotos_Path);

                if (!Directory.Exists(resources.Data_InstagramPhotos))
                    Directory.CreateDirectory(resources.Data_InstagramPhotos);

                if (!Directory.Exists(resources.Data_FacebookPhotos))
                    Directory.CreateDirectory(resources.Data_FacebookPhotos);

                if (!Directory.Exists(resources.PixelXmlMap_Path))
                    Directory.CreateDirectory(resources.PixelXmlMap_Path);

                if (!Directory.Exists(resources.PixelXmlMap_Path))
                    Directory.CreateDirectory(resources.PixelXmlMap_Path);

                if (!Directory.Exists(Path.Combine(resources.Data_Path, PixFormat._94x94.ToString())))
                    Directory.CreateDirectory(Path.Combine(resources.Data_Path, PixFormat._94x94.ToString()));

                if (!Directory.Exists(resources.Data_InstagramPhotos))
                {
                    Directory.CreateDirectory(resources.Data_InstagramPhotos);
                }

                if (!File.Exists(Path.Combine(resources.Data_InstagramPhotos, resources.BlackJPG)))
                {
                    string ss = Path.Combine(resources.Startup_Path, resources.BlackJPG);
                    string ff = Path.Combine(resources.Data_InstagramPhotos, resources.BlackJPG);
                    File.Copy(ss, ff);
                }

                if (!File.Exists(Path.Combine(resources.Startup_Path, resources.InQueueList)))
                {
                    FileStream fs = File.Create(Path.Combine(resources.Startup_Path, resources.InQueueList));
                    fs.Close();
                }
            }
            catch (Exception e)
            {
            }
        }

        public static bool CreateXml(string UserName)
        {
            qprPath resources = new qprPath(UserName);
            try
            {
                string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);
                if (file.Length != 1)
                {
                    string _path = Path.Combine(resources.Current_User, resources.UserXmlInfo);

                    XDocument belge = new XDocument();
                    XElement root = new XElement("_" + resources.UserName);

                    XElement ticket = new XElement("Ticket");
                    ticket.Value = "100000";
                    root.Add(ticket);

                    XElement busy = new XElement("Busy");
                    busy.Value = "0";
                    root.Add(busy);

                    XElement InstagramP = new XElement("InstagramPhotos");
                    XElement photo = new XElement("Photos", new XAttribute("useThis", false.ToString()));
                    photo.Value = "https://localhost/black.jpg";
                    InstagramP.Add(photo);

                    root.Add(InstagramP);

                    belge.Add(root);
                    belge.Save(_path);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static void XmlUpdate(string UserName, string key, int _Value, bool ValueArtir)
        {
            try
            {
                qprPath resources = new qprPath(UserName);
                string _path = Path.Combine(resources.Current_User, resources.UserXmlInfo);
                if (!File.Exists(_path))
                    throw new Exception("Hatalı kullanıcı bilgisi var.");

                XDocument belge = XDocument.Load(_path);
                XElement root = belge.Elements("_" + resources.UserName).First();
                var rootval = from p in root.Elements() where p.Element(key) == null select p;
                if (rootval == null)
                {
                    XElement Piro = new XElement(key);
                    if (ValueArtir)
                    {
                        int val = int.Parse(Piro.Value);
                        Piro.Value = (val + _Value).ToString();
                    }
                    else
                    {
                        Piro.Value = _Value.ToString();
                    }
                    root.Add(Piro);
                }
                else
                {
                    XElement Piro = root.Element(key);
                    if (ValueArtir)
                    {
                        int val = int.Parse(Piro.Value);
                        Piro.Value = (val + int.Parse(_Value.ToString())).ToString();
                    }
                    else
                    {
                        Piro.Value = _Value.ToString();
                    }
                }
                belge = new XDocument();
                belge.Add(root);
                belge.Save(_path);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int XmlGetValue(string UserName, string key)
        {
            qprPath resources = new qprPath(UserName);
            string _path = Path.Combine(resources.Current_User, resources.UserXmlInfo);
            if (!File.Exists(_path))
                throw new Exception("Hatalı kullanıcı bilgisi var.") { Source = "Ticket3" };

            XDocument belge = XDocument.Load(_path);
            XElement root = belge.Elements("_" + resources.UserName).First();
            XElement val = root.Element(key);
            if (val != null)
            {
                try
                {
                    return int.Parse(val.Value);
                }
                catch (Exception e)
                {
                    throw new Exception("Bilet değeri okunamıyor") { Source = "Ticket4" }; ;
                }
            }
            else
            {
                return -3;
            }
        }

        public static NewImagePart PointGenerator(string UserName, byte[] ImagePart, int x, int y, int width, int height, int PxFormat)
        {
            return ImageProperty.PointGenerator(UserName, ImagePart, x, y, width, height, PxFormat);
        }

        public static QpiroJSON XmlInstaPhotos(string UserName, XElement _InstagramP)
        {
            qprPath resources = new qprPath(UserName);
            QpiroJSON resp = new QpiroJSON();
            List<XElement> potos = _InstagramP.Elements("Photos").ToList();
            if (potos.Count > 0)
            {
                foreach (XElement item in potos)
                {
                    string usethis = item.Attribute("useThis").Value.ToLower();
                    string imgname = item.Value;
                    if (imgname.ToLower().IndexOf("/black.jpg") != -1)
                    {
                        continue;
                    }
                    bool b = false;
                    if (usethis == "true")
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    resp.Data.Add(new InstagramProfile.InstaPhoto()
                    {
                        UseThis = b,
                        name = imgname
                    });
                }
                potos.Clear();
            }
            else
            {
                resp.Message = "Instagram resiminiz bulunmamaktadır.";
            }
            return resp;
        }

        public static QpiroJSON XmlInstaPhotos(string UserName, string file)
        {
            qprPath resources = new qprPath(UserName);
            QpiroJSON resp = new QpiroJSON();
            XDocument doc = XDocument.Load(file);
            XElement root = doc.Elements("_" + resources.UserName).First();
            resp = XmlInstaPhotos(resources.UserName, root.Elements("InstagramPhotos").FirstOrDefault());
            doc = null;
            root = null;
            return resp;
        }

        public static bool DownloadInstaPhotos(string UserName, string[] photos)
        {
            qprPath resources = new qprPath(UserName);
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);

                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri.");

                string _range = "0-1000";// ilk kaç resim ?
                int min = int.Parse(_range.Split('-')[0]);
                int max = int.Parse(_range.Split('-')[1]);

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + resources.UserName).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                List<XElement> potos = InstagramP.Elements("Photos").ToList();
                //download
                List<string> imglist = photos.ToList();// InstagramProfile.UserPhotos();

                if (imglist.Count > 0)
                {
                    //https://scontent.cdninstagram.com/hphotos-xaf1/t51.2885-15/s150x150/e15/
                    for (int i = 0; i < imglist.Count; i++)
                    {
                        if (i >= min && i <= max)
                        {
                            string filname = Path.GetFileName(imglist[i]);
                            if (potos.FindIndex(p => Path.GetFileName(p.Value) == filname) == -1)
                            {
                                Bitmap btm = InstagramProfile.DownloadImage(imglist[i]);
                                if (btm == null)
                                    continue;

                                XElement photo = new XElement("Photos",
                                    new XAttribute("useThis", true.ToString())
                                    );
                                photo.Value = imglist[i];
                                InstagramP.Add(photo);

                                
                                using (MagickImage mini = new MagickImage(btm))
                                {
                                    mini.Quality = 100;
                                    mini.Resize(94, 94);

                                    mini.Write(Path.Combine(resources.Data_InstagramPhotos, filname));
                                    mini.Dispose();
                                }
                            }
                        }
                    }
                    doc.Save(file[0]);
                    resp.Data.Add("Resimler Güncellendi");
                }
                else
                {
                    throw new Exception("Instagram hesabınızda hiç fotoğrafınız bulunmuyor");
                }
                InstagramP = null;
                potos.Clear();
                imglist.Clear();
                return true;
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
                return false;
            }
        }

        public static QpiroJSON GetInstaPhotos(string UserName)
        {
            qprPath resources = new qprPath(UserName);
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);
                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri.");

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + resources.UserName).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();

                resp = XmlInstaPhotos(UserName, InstagramP);
                InstagramP = null;
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return resp;
        }

        public static QpiroJSON SelectedInstagramPhotos(string UserName)
        {
            qprPath resources = new qprPath(UserName);
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);
                string lst2 = HttpContext.Current.Request.Form[0];

                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                InstagramProfile.InstaPhoto[] outObject = JsonConvert.DeserializeObject<InstagramProfile.InstaPhoto[]>(lst2, serSettings);

                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri");

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + resources.UserName).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                IEnumerable<XElement> photos = InstagramP.Elements("Photos");

                if (outObject.Count() > 0)
                {
                    foreach (XElement photo in photos)
                    {
                        foreach (InstagramProfile.InstaPhoto selectedPhoto in outObject)
                        {
                            if (photo.Value == selectedPhoto.name)
                            {
                                XAttribute useths = photo.Attribute("useThis");
                                useths.Value = selectedPhoto.UseThis.ToString();
                                break;
                            }
                        }
                    }
                    doc.Save(file[0]);
                }
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return resp;
        }
    }
}