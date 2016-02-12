using Api._QPR;
using Api._QPR.abstracts;
using Api.Models;
using ImageMagick;
using Muuzy.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml.Linq;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ActionController : ApiController
    {
        //
        [Authorize]
        [HttpGet]
        [Route("users/instagram/images/download")]
        public IHttpActionResult DownloadInstaPhotos()
        {
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(UserProperty.Current_User, UserProperty.UserXmlInfo);

                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri.");

                string _range = "0-1000";// ilk kaç resim ?
                int min = int.Parse(_range.Split('-')[0]);
                int max = int.Parse(_range.Split('-')[1]);

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + ImageProperty.GetUserName()).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                List<XElement> potos = InstagramP.Elements("Photos").ToList();
                //download
                List<string> imglist = InstagramProfile.UserPhotos();

                ServerAsyncCallBack servers = new ServerAsyncCallBack();
                servers.Execute(Api._QPR.Type.AsyncCallType.DownloadInstaPhotos, imglist);

                if (imglist.Count > 0)
                {
                    //https://scontent.cdninstagram.com/hphotos-xaf1/t51.2885-15/s150x150/e15/
                    for (int i = 0; i < imglist.Count; i++)
                    {
                        if (i >= min && i <= max)
                        {
                            //resp.Data.Add(imglist[i]);
                            string filname = Path.GetFileName(imglist[i]);
                            if (potos.FindIndex(p => Path.GetFileName(p.Value) == filname) == -1)
                            {
                                XElement photo = new XElement("Photos",
                                    new XAttribute("useThis", true.ToString())
                                    );
                                photo.Value = imglist[i];
                                InstagramP.Add(photo);

                                using (MagickImage mini = new MagickImage(InstagramProfile.DownloadImage(imglist[i])))
                                {
                                    mini.Quality = 100;
                                    mini.Resize(94, 94);

                                    mini.Write(Path.Combine(UserProperty.Data_InstagramPhotos, filname));
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
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("users/instagram/images/get")]
        public IHttpActionResult GetInstaPhotos()
        {
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(UserProperty.Current_User, UserProperty.UserXmlInfo);
                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri.");

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + ImageProperty.GetUserName()).First();
                XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                resp = UserProperty.XmlInstaPhotos(InstagramP);
                InstagramP = null;
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("images/formats")]
        public IHttpActionResult ImageFormats()
        {
            QpiroJSON resp = new QpiroJSON();

            #region tüm formatları getir

            string[] listName = Enum.GetNames(typeof(PixFormat));

            foreach (var item in listName)
            {
                if (item == "_null" /*|| item == "_12x12" || item == "_20x20"*/ || item == "_36x36")
                    continue;
                resp.Data.Add(item);
            }

            #endregion tüm formatları getir

            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("images/generate")]
        public IHttpActionResult ImageGenerate()
        {
            QpiroJSON resp = new QpiroJSON();
            UserProperty.ImgType itype = new UserProperty.ImgType();
            PixFormat PixelFormat = new PixFormat();
            string imgName = "";
            try
            {
                ///////////////////////////////////////////////
                int bsy = UserProperty.XmlGetValue("Busy");
                if (bsy == 1)
                    throw new Exception("Bir işlem halen devam etmekte bitmesini bekleyiniz.") { Source = "Busy" };
                ///////////////////////////////////////////////////
                int tck = UserProperty.XmlGetValue("Ticket");
                if (tck == -3)
                {
                    throw new Exception("Kullanıcı bilgileri bulunamadı.") { Source = "Ticket1" };
                }
                else
                {
                    if (!UserProperty.EmailVerified)
                        throw new Exception("Eposta adresinizi onaylamanız gerekmektedir") { Source = "EVerified" };

                    if (-1 >= tck - 1)
                    {
                        throw new Exception("Biletiniz bitmiş işlem yapamıyoruz.") { Source = "Ticket2" };
                    }
                    else
                    {
                        UserProperty.XmlUpdate("Ticket", -1, true);
                        UserProperty.XmlUpdate("Busy", 1, false);
                    }
                }
                ////////////////////////////////////////////////////
                imgName = Convert.ToString(HttpContext.Current.Request["imagename"]);
                itype = UserProperty.ImgType.Resources;
                string typ = HttpContext.Current.Request["pixtype"];
                if (typ == null)
                    throw new Exception("Lütfen format seçiniz") { Source = "" };

                PixelFormat = (PixFormat)ImageProperty.StringToEnum(typeof(PixFormat), Convert.ToString(typ));
            }
            catch (Exception e)
            {
                if (e.Source == "Ticket1" ||
                    e.Source == "Ticket2" ||
                    e.Source == "Ticket3" ||
                    e.Source == "Ticket4" ||
                    e.Source == "Busy" ||
                    e.Source == "EVerified")
                { }
                else
                {
                    UserProperty.XmlUpdate("Ticket", 1, true);
                    UserProperty.XmlUpdate("Busy", 0, false);
                }
                resp.Message = e.Message;
                return this.Json<QpiroJSON>(resp);
            }
            try
            {
                string imgPath = "";
                switch (itype)
                {
                    case UserProperty.ImgType.Resources:
                        imgPath = Path.Combine(UserProperty.ResourcePhotos_Path, imgName + ImageProperty.JPG);
                        break;

                    default:
                        throw new Exception("Tanımlanamayan imagename formatı") { Source = "" };
                        break;
                }
                if (File.Exists(imgPath))
                {
                    DateTime start = DateTime.Now;
                    System.GC.Collect();
                    List<PartOfImage> imgsInf = ImageProperty.PartOfImage(imgPath, imgName, (int)PixelFormat);
                    ServerAsyncCallBack servers = new ServerAsyncCallBack();
                    servers.Execute(_QPR.Type.AsyncCallType.ImageGenerate, imgsInf, (int)PixelFormat);
                    imgsInf.Clear();
                    //do
                    //{
                    //    Thread.Sleep(1);
                    //} while (servers._respLocalTest.Status == TaskStatus.WaitingForActivation);
                    do
                    {
                        Thread.Sleep(999);
                    } while (
                    servers._resp1.Status == TaskStatus.WaitingForActivation ||
                    servers._resp2.Status == TaskStatus.WaitingForActivation ||
                    servers._resp3.Status == TaskStatus.WaitingForActivation ||
                    servers._resp4.Status == TaskStatus.WaitingForActivation ||
                    servers._resp5.Status == TaskStatus.WaitingForActivation ||
                    servers._resp6.Status == TaskStatus.WaitingForActivation ||
                    servers._resp7.Status == TaskStatus.WaitingForActivation ||
                    servers._resp8.Status == TaskStatus.WaitingForActivation
                    );
                    DateTime end = DateTime.Now;
                    string time = TimeSpan.FromTicks(end.Ticks - start.Ticks).ToString();
                    resp.Time = (time.Split(':')[time.Split(':').Length - 2] + ":" + time.Split(':')[time.Split(':').Length - 1]);
                    ///////////////////////////////////////
                    //byte[] bitmp0 = servers._respLocalTest.Result.Body.ImageGenerateResult.newImage;
                    //Rectangle br0 = ImageProperty.stringToRectangle(servers._respLocalTest.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp1 = servers._resp1.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br1 = ImageProperty.stringToRectangle(servers._resp1.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp2 = servers._resp2.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br2 = ImageProperty.stringToRectangle(servers._resp2.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp3 = servers._resp3.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br3 = ImageProperty.stringToRectangle(servers._resp3.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp4 = servers._resp4.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br4 = ImageProperty.stringToRectangle(servers._resp4.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp5 = servers._resp5.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br5 = ImageProperty.stringToRectangle(servers._resp5.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp6 = servers._resp6.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br6 = ImageProperty.stringToRectangle(servers._resp6.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp7 = servers._resp7.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br7 = ImageProperty.stringToRectangle(servers._resp7.Result.Body.ImageGenerateResult.ImagePartInfo);
                    byte[] bitmp8 = servers._resp8.Result.Body.ImageGenerateResult.newImage;
                    Rectangle br8 = ImageProperty.stringToRectangle(servers._resp8.Result.Body.ImageGenerateResult.ImagePartInfo);

                    servers = null;
                    int newW = 0, newH = 0;
                    if (br1.Width < br1.Height)
                    {
                        newW = br1.Width * UserProperty.ComputerNumber;
                        newH = br1.Height;
                    }
                    else
                    {
                        newW = br1.Width;
                        newH = br1.Height * UserProperty.ComputerNumber;
                    }
                    //MagickImage img0 = new MagickImage(bitmp0);

                    MagickImage img1 = new MagickImage(bitmp1);
                    MagickImage img2 = new MagickImage(bitmp2);
                    MagickImage img3 = new MagickImage(bitmp3);
                    MagickImage img4 = new MagickImage(bitmp4);
                    MagickImage img5 = new MagickImage(bitmp5);
                    MagickImage img6 = new MagickImage(bitmp6);
                    MagickImage img7 = new MagickImage(bitmp7);
                    MagickImage img8 = new MagickImage(bitmp8);

                    Bitmap newImg = new Bitmap(newW, newH);
                    Graphics grr = Graphics.FromImage(newImg);
                    //grr.DrawImage(img0.ToBitmap(), br0.X, br0.Y);
                    grr.DrawImage(img1.ToBitmap(), br1.X, br1.Y);
                    grr.DrawImage(img2.ToBitmap(), br2.X, br2.Y);
                    grr.DrawImage(img3.ToBitmap(), br3.X, br3.Y);
                    grr.DrawImage(img4.ToBitmap(), br4.X, br4.Y);
                    grr.DrawImage(img5.ToBitmap(), br5.X, br5.Y);
                    grr.DrawImage(img6.ToBitmap(), br6.X, br6.Y);
                    grr.DrawImage(img7.ToBitmap(), br7.X, br7.Y);
                    grr.DrawImage(img8.ToBitmap(), br8.X, br8.Y);
                    grr.Dispose();
                    (resp.Data as List<object>).Add(ImageProperty.ImageToBase64(newImg, System.Drawing.Imaging.ImageFormat.Jpeg));
                    ///////////////////////////////////////
                    UserProperty.XmlUpdate("Busy", 0, false);
                    time = "";
                }
                else
                {
                    throw new Exception("Belirtilen resim bulunamadı") { Source = "" };
                }
            }
            catch (Exception e)
            {
                UserProperty.XmlUpdate("Busy", 0, false);
                UserProperty.XmlUpdate("Ticket", 1, true);
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("images/get")]
        public IHttpActionResult ImageGet()
        {
            QpiroJSON resp = new QpiroJSON();
            UserProperty.ImgType _itype;
            try
            {
                string val = HttpContext.Current.Request["type"];
                try
                {
                    _itype = (UserProperty.ImgType)ImageProperty.StringToEnum(typeof(UserProperty.ImgType), val);
                }
                catch
                {
                    throw new Exception("Bu resim tipi bulunamadı");
                }
                string[] list = new string[0];
                string img_path = "";
                if (_itype == UserProperty.ImgType.Resources)
                    img_path = UserProperty.ResourcePhotos_Path;
                else if (_itype == UserProperty.ImgType.Saved)
                    img_path = UserProperty.SavedPhotos_Path;
                else if (_itype == UserProperty.ImgType.Mini)
                    img_path = Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString());
                else if (_itype == UserProperty.ImgType.InstaMini)
                {
                    string[] file = Directory.GetFiles(UserProperty.Current_User, UserProperty.UserXmlInfo);

                    if (file.Count() != 1)
                        throw new Exception("Geçersiz kullanıcı bilgileri");

                    resp = UserProperty.XmlInstaPhotos(file[0]);

                    return this.Json<QpiroJSON>(resp);
                }
                if (!Directory.Exists(img_path))
                    throw new Exception("Kullanıcı bilgileri hatalı");

                list = Directory.GetFiles(img_path, "*" + ImageProperty.JPG);
                foreach (string item in list)
                {
                    string path = "/userfolders" + item.ToLower().Split(new string[] { "userfolders" }, StringSplitOptions.None)[1].Replace("\\", "/");
                    resp.Data.Add(Path.GetFileName(path).Replace(".jpg", ""));
                }
                list = new string[0];
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("images/remove")]
        public IHttpActionResult ImageRemove()
        {
            QpiroJSON resp = new QpiroJSON();
            string valImgs = HttpContext.Current.Request["names"];
            string ImgType = HttpContext.Current.Request["type"];
            UserProperty.ImgType _itype;
            try
            {
                try
                {
                    _itype = (UserProperty.ImgType)ImageProperty.StringToEnum(typeof(UserProperty.ImgType), ImgType);
                }
                catch (Exception)
                {
                    throw new Exception("Bu resim tipi bulunamadı");
                }
                string img_path = "";

                #region diğer formatları silme

                //if (_itype == UsrImageProc.ImgType.Resources)
                //    img_path = UsrImageProc.ResourcePhotos_Path + "\\";
                //else if (_itype == UsrImageProc.ImgType.Saved)
                //    img_path = UsrImageProc.SavedPhotos_Path + "\\";
                //else

                #endregion diğer formatları silme

                if (_itype == UserProperty.ImgType.Mini)
                    img_path = Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString());
                if (!Directory.Exists(img_path))
                    throw new Exception("Kullanıcı bilgileri hatalı");

                List<string> imgs = Directory.GetFiles(img_path, "*" + ImageProperty.JPG).ToList();
                string[] remvimgs = valImgs.Replace(",", ImageProperty.JPG + ",").Split(',');//jpg,
                int basarili = 0, basarisiz = 0;
                for (int i = 0; i < remvimgs.Length; i++)
                {
                    int index = imgs.FindIndex(p => Path.GetFileName(p) == remvimgs[i]);
                    if (index != -1)
                    {
                        try
                        {
                            File.Delete(imgs[index]);
                            basarili++;
                        }
                        catch
                        {
                            basarisiz++;
                        }
                    }
                }
                Dictionary<string, int> sonuc = new Dictionary<string, int>();
                if (basarili == 0 && basarisiz == 0)
                    resp.Message = "eşleşen sonuç yok";
                else
                {
                    sonuc.Add("silinen", basarili);
                    sonuc.Add("silinemeyen", basarisiz);
                }
                resp.Data.Add(sonuc);
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpGet]
        [Route("images/show")]
        public IHttpActionResult ImagesShow()
        {
            QpiroJSON resp = new QpiroJSON();
            string img_path = "";
            string valImg = HttpContext.Current.Request["name"];
            string valTyp = HttpContext.Current.Request["type"];
            try
            {
                if (valImg == null || valTyp == null)
                    throw new Exception("Parametreler geçersiz.");

                UserProperty.ImgType itype = new UserProperty.ImgType();
                try
                {
                    itype = (UserProperty.ImgType)ImageProperty.StringToEnum(typeof(UserProperty.ImgType), valTyp);
                }
                catch
                {
                    throw new Exception("Tip doğrulanamadı.");
                }

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                if (itype == UserProperty.ImgType.Resources)
                    img_path = UserProperty.ResourcePhotos_Path;
                else if (itype == UserProperty.ImgType.Saved)
                    img_path = UserProperty.SavedPhotos_Path;
                else if (itype == UserProperty.ImgType.Mini)
                    img_path = Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString());

                if (!Directory.Exists(img_path))
                    throw new Exception("Mini dosya yolu doğrulanamadı.");

                img_path = Path.Combine(img_path, valImg + ImageProperty.JPG);

                if (!File.Exists(img_path))
                    throw new Exception("Mini dosya bulunamadı.");
                Bitmap btm = new Bitmap(img_path);
                string val = ImageProperty.ImageToBase64(btm, System.Drawing.Imaging.ImageFormat.Jpeg);
                resp.Data.Add(val);
                btm.Dispose();
                val = null;
            }
            catch (Exception e)
            {
                resp.Message = e.Message;
            }
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpPost]
        [Route("users/instagram/images/update")]
        public IHttpActionResult SelectedInstaPhotos([FromBody]string ls)
        {
            QpiroJSON resp = new QpiroJSON();
            try
            {
                string[] file = Directory.GetFiles(UserProperty.Current_User, UserProperty.UserXmlInfo);
                string lst2 = HttpContext.Current.Request.Form[0];

                JsonSerializerSettings serSettings = new JsonSerializerSettings();
                serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                InstagramProfile.InstaPhoto[] outObject = JsonConvert.DeserializeObject<InstagramProfile.InstaPhoto[]>(lst2, serSettings);

                if (file.Count() != 1)
                    throw new Exception("Geçersiz kullanıcı bilgileri");

                XDocument doc = XDocument.Load(file[0]);
                XElement root = doc.Elements("_" + ImageProperty.GetUserName()).First();
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
            return this.Json<QpiroJSON>(resp);
        }

        [Authorize]
        [HttpPost]
        [Route("uploads/resource")]
        public void UploadResource()
        {
            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
                try
                {
                    HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files["file"];
                    string fileExt = Path.GetExtension(httpPostedFile.FileName).ToLower();
                    if (fileExt != ".jpg" || (httpPostedFile.ContentLength / 1024) > 12192)
                        throw new Exception("Dosya uzantısı geçerli değil veya boyut 12mb dan büyük.");
                    string SaveFileDir = "";
                    string randomhexnum = "";
                    do
                    {
                        randomhexnum = "1" + ImageProperty.GenerateRandomHexNumber(6) + "a";
                        SaveFileDir = Path.Combine(UserProperty.ResourcePhotos_Path, randomhexnum + fileExt);
                    }
                    while (File.Exists(SaveFileDir));
                    using (MagickImage imagem = new MagickImage(httpPostedFile.InputStream))
                    {
                        int yuzde = 50;
                        imagem.Quality = 100;
                        int _w = imagem.Width + (imagem.Width / 100) * yuzde;
                        int _h = imagem.Height + (imagem.Height / 100) * yuzde;
                        imagem.Resize(_w, _h);
                        imagem.Write(SaveFileDir);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        [Authorize]
        [HttpGet]
        [Route("users/logged")]
        public bool UsersLogged()
        {
            bool snc;
            snc = UserProperty.CreateDir();
            snc = UserProperty.XmlCreate();
            return snc;
        }

        //[Authorize]
        //[HttpPost]
        //[Route("uploads/mini")]
        //public void UploadMini()
        //{
        //    if (HttpContext.Current.Request.Files.AllKeys.Any())
        //    {
        //        try
        //        {
        //            HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files["file"];
        //            string fileExt = Path.GetExtension(httpPostedFile.FileName).ToLower();
        //            if (fileExt != ".jpg" || (httpPostedFile.ContentLength / 1024) > 9192)
        //                return;
        //            string fileSavePath = "";
        //            string randomhexnum = "";
        //            do
        //            {
        //                randomhexnum = "_" + ImageProperty.GenerateRandomHexNumber(10);
        //                fileSavePath = Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString(), randomhexnum + fileExt);
        //            }
        //            while (File.Exists(fileSavePath));
        //            Bitmap btm = (Bitmap)ImageProperty.resizeImage(Bitmap.FromStream(httpPostedFile.InputStream), new Size(94, 94));
        //            btm.Save(fileSavePath);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception(e.Message);
        //        }
        //    }
        //}
    }
}