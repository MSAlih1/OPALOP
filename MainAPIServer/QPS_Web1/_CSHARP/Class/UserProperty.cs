using Api;
using Api._QPR;
using Api._QPR.abstracts;
using Api.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Xml.Linq;

namespace Muuzy.Class
{
    public static class UserProperty
    {
        public static string Startup_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFolders");

        public enum ImgType
        {
            Resources = 0,
            Mini = 1,
            Saved = 2,
            Current = 3,
            InstaMini = 4,
        }

        public enum ProcType
        {
            ImageLocation = 0,
            OrginalImage = 1,
            FormatType = 2,
            UserName = 3,
            ServIP = 4
        }

        public static string BlackJPG
        {
            get { return "black.jpg"; }
        }

        public static int ComputerNumber { get { return 1; } }

        public static string Current_User
        { get { return Path.Combine(Startup_Path, UserName); } }

        public static string Data_FacebookPhotos
        { get { return Path.Combine(Data_Path, PixFormat._94x94.ToString(), "Facebook"); } }

        public static string Data_InstagramPhotos
        { get { return Path.Combine(Data_Path, PixFormat._94x94.ToString(), "Instagram"); } }

        public static string Data_Path
        { get { return Path.Combine(Current_User, "Data"); } }

        public static bool EmailVerified
        {
            get
            {
                //Client cn = UserProperty.ConnectAuth();
                //UserProfile up = cn.GetUser(ClaimsPrincipal.Current.Identity.Name);
                //string isValid = up.ExtraProperties["email_verified"].ToString();
                //if (isValid == "True")
                //{
                //    return true;
                //}
                //else
                //{
                return true;
                //}
            }
        }

        public static string InQueueList
        {
            get { return "InQueueList.txt"; }
        }

        public static string PixelXmlMap_Path
        { get { return Path.Combine(Data_Path, "PixelXmlMap"); } }

        public static string ResourcePhotos_Path
        { get { return Path.Combine(Data_Path, "ResourcePhotos"); } }

        public static string SavedPhotos_Path
        { get { return Path.Combine(Data_Path, "SavedPhotos"); } }

        public static string ServerIP
        {
            get
            { return "api.qpiro.com"; }
        }
        public static string UserName
        { get { return ImageProperty.GetUserName(); } }

        public static string UserXmlInfo
        {
            get { return "UserInfo.xml"; }
        }
        public static bool CreateDir()
        {
            try
            {
                if (!Directory.Exists(UserProperty.ResourcePhotos_Path))
                    Directory.CreateDirectory(UserProperty.ResourcePhotos_Path);

                if (!Directory.Exists(UserProperty.SavedPhotos_Path))
                    Directory.CreateDirectory(UserProperty.SavedPhotos_Path);

                if (!Directory.Exists(UserProperty.Data_InstagramPhotos))
                    Directory.CreateDirectory(UserProperty.Data_InstagramPhotos);

                if (!Directory.Exists(UserProperty.Data_FacebookPhotos))
                    Directory.CreateDirectory(UserProperty.Data_FacebookPhotos);

                if (!Directory.Exists(UserProperty.PixelXmlMap_Path))
                    Directory.CreateDirectory(UserProperty.PixelXmlMap_Path);

                if (!Directory.Exists(UserProperty.PixelXmlMap_Path))
                    Directory.CreateDirectory(UserProperty.PixelXmlMap_Path);

                if (!Directory.Exists(Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString())))
                    Directory.CreateDirectory(Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString()));

                if (!Directory.Exists(UserProperty.Data_InstagramPhotos))
                {
                    Directory.CreateDirectory(UserProperty.Data_InstagramPhotos);
                }

                if (!File.Exists(Path.Combine(UserProperty.Data_InstagramPhotos, UserProperty.BlackJPG)))
                {
                    File.Copy(
                        Path.Combine(UserProperty.Startup_Path, UserProperty.BlackJPG),
                        Path.Combine(UserProperty.Data_InstagramPhotos, UserProperty.BlackJPG)
                        );
                }

                if (!File.Exists(Path.Combine(UserProperty.Startup_Path, InQueueList)))
                {
                    FileStream fs = File.Create(Path.Combine(UserProperty.Startup_Path, InQueueList));
                    fs.Close();
                }
                ServerAsyncCallBack servers = new ServerAsyncCallBack();
                servers.Execute(Api._QPR.Type.AsyncCallType.CreateDir);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool XmlCreate()
        {
            try
            {
                string[] file = Directory.GetFiles(UserProperty.Current_User, UserProperty.UserXmlInfo);
                if (file.Length != 1)
                {
                    string _path = Path.Combine(UserProperty.Current_User, UserProperty.UserXmlInfo);

                    XDocument belge = new XDocument();
                    XElement root = new XElement("_" + ImageProperty.GetUserName());

                    XElement ticket = new XElement("Ticket");
                    ticket.Value = "100";
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

                ServerAsyncCallBack servers = new ServerAsyncCallBack();
                servers.Execute(Api._QPR.Type.AsyncCallType.CreateXml);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static int XmlGetValue(string key)
        {
            string _path = Path.Combine(UserProperty.Current_User, UserProperty.UserXmlInfo);
            if (!File.Exists(_path))
                throw new Exception("Hatalı kullanıcı bilgisi var.") { Source = "Ticket3" };

            XDocument belge = XDocument.Load(_path);
            XElement root = belge.Elements("_" + ImageProperty.GetUserName()).First();
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

        public static QpiroJSON XmlInstaPhotos(XElement _InstagramP)
        {
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

        public static QpiroJSON XmlInstaPhotos(string file)
        {
            QpiroJSON resp = new QpiroJSON();
            XDocument doc = XDocument.Load(file);
            XElement root = doc.Elements("_" + ImageProperty.GetUserName()).First();
            resp = XmlInstaPhotos(root.Elements("InstagramPhotos").FirstOrDefault());
            doc = null;
            root = null;
            return resp;
        }

        public static void XmlUpdate(string key, int _Value, bool ValueArtir)
        {
            try
            {
                string _path = Path.Combine(UserProperty.Current_User, UserProperty.UserXmlInfo);
                if (!File.Exists(_path))
                    throw new Exception("Hatalı kullanıcı bilgisi var.");

                XDocument belge = XDocument.Load(_path);
                XElement root = belge.Elements("_" + ImageProperty.GetUserName()).First();
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
    }
}