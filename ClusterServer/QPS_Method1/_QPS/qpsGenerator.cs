using ImageMagick;
using QPS_Web1._CSHARP.Class;
using QPS_Web1._QPS.Class;
using QPS_Web1._QPS.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

namespace QPS_Web1._QPS
{
    public static class qpsGenerator
    {
        public static Random random = new Random();

        public static string PointGenerator(string getusername, int _Startx, int _Start_y, int End_x, int End_y, string inputBitmapPath, PixFormat _pixFor)
        {
            List<string> imglist = new List<string>();
            try
            {
                qprPath resources = new qprPath(getusername);

                if (_pixFor == PixFormat._null)
                    throw new Exception("format null olamaz") { Source = "" };

                using (MagickImage imagem = new MagickImage(inputBitmapPath))
                {
                    //############# ResizeImage ############
                    int yuzde = 20;
                    imagem.Quality = 100;
                    int _w = imagem.Width + (imagem.Width / 100) * yuzde;
                    int _h = imagem.Height + (imagem.Height / 100) * yuzde;
                    int new_W = _w - (_w % (int)_pixFor);
                    int new_H = _h - (_h % (int)_pixFor);
                    imagem.Resize(new_W, new_H);
                    imagem.Blur(5, 5);
                    //############# GenerateSquare #############
                    /////////// calculate image point rgb avg   ////////////
                    string curImgPth = Path.GetFileName(inputBitmapPath);
                    string path = resources.PixelXmlMap_Path + "\\" + curImgPth.Substring(0, curImgPth.Length - 4) + _pixFor.ToString() + "_.xml";
                    FileInfo finf = new FileInfo(path);
                    List<ImgSquare> sp0 = new List<ImgSquare>();
                    if (!File.Exists(path))
                    {
                        int[,] pixavg = new int[5, 3];
                        int _pixformat = (int)_pixFor;
                        WritablePixelCollection _totalpix = imagem.GetWritablePixels(0, 0, imagem.Width, imagem.Height);
                        int range = _pixformat / 2;
                        for (int w = 0; w < imagem.Width; w += _pixformat)
                        {
                            for (int h = 0; h < imagem.Height; h += _pixformat)
                            {
                                if (!(w + _pixformat <= imagem.Width && h + _pixformat <= imagem.Height))
                                    continue;//olmazda olursa diye
                                pixavg = new int[5, 3];
                                for (int x = 0; x < range; x++)
                                {
                                    for (int y = 0; y < range; y++)
                                    {
                                        Color a = _totalpix.GetPixel(x + w + 0, h + 0 + y).ToColor().ToColor();
                                        pixavg[0, 0] += a.R;
                                        pixavg[0, 1] += a.G;
                                        pixavg[0, 2] += a.B;

                                        Color b = _totalpix.GetPixel(x + w + range, h + y).ToColor().ToColor();
                                        pixavg[1, 0] += b.R;
                                        pixavg[1, 1] += b.G;
                                        pixavg[1, 2] += b.B;

                                        Color c = _totalpix.GetPixel(x + w, h + range + y).ToColor().ToColor();
                                        pixavg[2, 0] += c.R;
                                        pixavg[2, 1] += c.G;
                                        pixavg[2, 2] += c.B;

                                        Color d = _totalpix.GetPixel(x + w + range, h + range + y).ToColor().ToColor();
                                        pixavg[3, 0] += d.R;
                                        pixavg[3, 1] += d.G;
                                        pixavg[3, 2] += d.B;
                                    }
                                }

                                //tümü için aynı toplanıyor
                                pixavg[4, 0] = pixavg[0, 0] + pixavg[1, 0] + pixavg[2, 0] + pixavg[3, 0];
                                pixavg[4, 1] = pixavg[0, 1] + pixavg[1, 1] + pixavg[2, 1] + pixavg[3, 1];
                                pixavg[4, 2] = pixavg[0, 2] + pixavg[1, 2] + pixavg[2, 2] + pixavg[3, 2];
                                //----

                                int totalminiPix = (range * range);
                                pixavg[0, 0] /= totalminiPix;
                                pixavg[0, 1] /= totalminiPix;
                                pixavg[0, 2] /= totalminiPix;

                                pixavg[1, 0] /= totalminiPix;
                                pixavg[1, 1] /= totalminiPix;
                                pixavg[1, 2] /= totalminiPix;

                                pixavg[2, 0] /= totalminiPix;
                                pixavg[2, 1] /= totalminiPix;
                                pixavg[2, 2] /= totalminiPix;

                                pixavg[3, 0] /= totalminiPix;
                                pixavg[3, 1] /= totalminiPix;
                                pixavg[3, 2] /= totalminiPix;

                                int totalPix = totalminiPix * 4;
                                pixavg[4, 0] /= totalPix;
                                pixavg[4, 1] /= totalPix;
                                pixavg[4, 2] /= totalPix;

                                sp0.Add(new ImgSquare(w, h, new List<QuardPixAvg>()
                            {
                                new QuardPixAvg(Color.FromArgb((pixavg[0, 0]), (pixavg[0, 1]), (pixavg[0, 2])), QuardBolum.SolUst),
                                new QuardPixAvg (Color.FromArgb((pixavg[1, 0]), (pixavg[1, 1]), (pixavg[1, 2])), QuardBolum.SagUst),
                                new QuardPixAvg(Color.FromArgb((pixavg[2, 0]), (pixavg[2, 1]), (pixavg[2, 2])), QuardBolum.SolAlt),
                                new QuardPixAvg(Color.FromArgb((pixavg[3, 0]), (pixavg[3, 1]), (pixavg[3, 2])), QuardBolum.SagAlt),
                                new QuardPixAvg(Color.FromArgb((pixavg[4, 0]), (pixavg[4, 1]), (pixavg[4, 2])), QuardBolum.TotalAvg)
                            }));
                            }
                        }
                        _totalpix = null;
                        pixavg = null;
                        //////////////////////////// xml generate ///////////////////////////////
                        XmlDocument doc = new XmlDocument();
                        XmlElement root = doc.CreateElement("SquarePoints");
                        root.SetAttribute("Count", sp0.Count.ToString());
                        root.SetAttribute("PixFormat", _pixFor.ToString());
                        foreach (ImgSquare item in sp0)
                        {
                            XmlElement child = doc.CreateElement("SquarePoint");
                            child.SetAttribute("WxH", item.ToString());
                            child.SetAttribute("ColorAVG", item.SAvgArb.ToString());
                            List<QuardPixAvg> lstQ = item.QuardAvg;
                            child.SetAttribute("SolUst", lstQ[0].QuardAvg.ToString());
                            child.SetAttribute("SagUst", lstQ[1].QuardAvg.ToString());
                            child.SetAttribute("SolAlt", lstQ[2].QuardAvg.ToString());
                            child.SetAttribute("SagAlt", lstQ[3].QuardAvg.ToString());
                            root.AppendChild(child);
                        }
                        doc.AppendChild(root);
                        if (!Directory.Exists(finf.Directory.FullName))
                            Directory.CreateDirectory(finf.Directory.FullName);

                        doc.Save(path);
                        doc = null;
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(path);
                        XmlNodeList root1 = doc.GetElementsByTagName("SquarePoints");
                        XmlNode root2 = root1.Item(0);
                        foreach (XmlNode item in root2.ChildNodes)
                        {
                            try
                            {
                                XmlAttribute at1 = item.Attributes[0];
                                string[] point = at1.InnerText.Split('x');
                                XmlAttribute at2 = item.Attributes[1];
                                string ColorAvg = at2.InnerText;
                                List<QuardPixAvg> lQuard = new List<QuardPixAvg>();
                                int SolUst = int.Parse(item.Attributes[2].InnerText);
                                int SagUst = int.Parse(item.Attributes[3].InnerText);
                                int SolAlt = int.Parse(item.Attributes[4].InnerText);
                                int SagAlt = int.Parse(item.Attributes[5].InnerText);
                                int Tumu = int.Parse(item.Attributes[1].InnerText);
                                lQuard.Add(new QuardPixAvg(Color.FromArgb(SolUst), QuardBolum.SolUst));
                                lQuard.Add(new QuardPixAvg(Color.FromArgb(SagUst), QuardBolum.SagUst));
                                lQuard.Add(new QuardPixAvg(Color.FromArgb(SolAlt), QuardBolum.SolAlt));
                                lQuard.Add(new QuardPixAvg(Color.FromArgb(SagAlt), QuardBolum.SagAlt));
                                lQuard.Add(new QuardPixAvg(Color.FromArgb(Tumu), QuardBolum.TotalAvg));
                                sp0.Add(new ImgSquare(int.Parse(point[0]), int.Parse(point[1]), lQuard));
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                        root2 = null;
                        root1 = null;
                        doc = null;
                    }
                    //#---------------------------------
                    int opacity = 50;
                    switch (_pixFor)
                    {
                        case PixFormat._null:
                            sp0.Clear();
                            throw new Exception("tanımsız format") { Source = "" };
                            break;

                        case PixFormat._12x12: //12:36 sn
                            opacity = 100;
                            break;

                        case PixFormat._20x20: //3:10 sn
                            opacity = 50;
                            break;

                        case PixFormat._36x36: //0:50 sn
                            opacity = 50;
                            break;

                        case PixFormat._48x48: //0:27 sn
                            opacity = 50;
                            break;

                        case PixFormat._64x64: //0:15 sn
                            opacity = 50;
                            break;

                        case PixFormat._94x94: //0:08 sn
                            opacity = 50;
                            break;
                    }
                    string MiniPicturePath = Path.Combine(resources.Data_Path, PixFormat._94x94.ToString());
                    if (Directory.Exists(MiniPicturePath))
                    {
                        Bitmap btm = imagem.ToBitmap();
                        using (Graphics gr1 = Graphics.FromImage(btm))//graphics işleme tekrar ayarla
                        {
                            string[] list = Directory.GetFiles(MiniPicturePath, "*" + ImageProperty.JPG);
                            if (list.Length == 0)
                            {
                                throw new Exception("Bu Formata Uygun Resimler Bulunamadı") { Source = "" };
                            }
                            List<ImgSquare> spl2 = new List<ImgSquare>();
                            for (int i = 0; i < list.Length; i++)
                            {
                                using (MagickImage mini = new MagickImage(list[i]))
                                {
                                    mini.Quality = 100;
                                    if (mini.Width != 94 && mini.Height != 94)
                                        mini.Resize((int)_pixFor, (int)_pixFor);

                                    spl2.Add(new ImgSquare(mini.ToBitmap()));
                                    mini.Dispose();
                                }
                            }

                            List<ImgSquare> spl4 = new List<ImgSquare>();
                            spl4.AddRange(sp0);
                            spl4.AddRange(spl2);
                            spl2.Clear();
                            //küçük resimlerin parpaklık oranları tespit edilp parlaklığı ayarlanacak
                            //büyük resim çok parlaksa koyulaştırılacak
                            //büyük resim çok karanlıksa aydınlatılacak
                            spl4 = spl4.OrderBy(p => p.GeneratedColorCode).ToList();
                            int qpiro_number = 900000;

                            for (int i = 0; i < sp0.Count; i++)
                            {
                                ImgSquare item = sp0[i];
                                try
                                {
                                    List<ImgSquare> snc = spl4.Where(p =>
                                 (p.GeneratedColorCode - qpiro_number < item.GeneratedColorCode &&
                                 p.GeneratedColorCode + qpiro_number > item.GeneratedColorCode) &&
                                 p.isArea == false).ToList();

                                    Rectangle rec = new Rectangle(item.W, item.H, (int)_pixFor, (int)_pixFor);
                                    if (snc.Count != 0)
                                    {
                                        int randi = random.Next(0, snc.Count);
                                        System.Drawing.Image img = ImageProperty.SmartRotate(item, snc[randi]);
                                        snc.Clear();//
                                        img = ImageProperty.Transparnt(img, opacity);
                                        gr1.DrawImage(img, rec);
                                        sp0.RemoveAt(i);
                                        i--;
                                    }
                                    else
                                    {
                                        snc.Clear();//
                                        snc = spl4.Where(p =>
                                          (p.GeneratedColorCode - (qpiro_number * 2) < item.GeneratedColorCode &&
                                          p.GeneratedColorCode + (qpiro_number * 2) > item.GeneratedColorCode) &&
                                          p.isArea == false).ToList();

                                        if (snc.Count != 0)
                                        {
                                            int randi = random.Next(0, snc.Count);
                                            System.Drawing.Image img = ImageProperty.SmartRotate(item, snc[randi]);
                                            snc.Clear();//
                                            img = ImageProperty.Transparnt(img, opacity);
                                            gr1.DrawImage(img, rec);
                                            sp0.RemoveAt(i);
                                            i--;
                                        }
                                        else
                                        {
                                            snc.Clear();//
                                            snc = spl4.Where(p =>
                                              (p.GeneratedColorCode - (qpiro_number * 3) < item.GeneratedColorCode &&
                                              p.GeneratedColorCode + (qpiro_number * 3) > item.GeneratedColorCode) &&
                                              p.isArea == false).ToList();

                                            if (snc.Count != 0)
                                            {
                                                int randi = random.Next(0, snc.Count);
                                                System.Drawing.Image img = ImageProperty.SmartRotate(item, snc[randi]);
                                                snc.Clear();//
                                                img = ImageProperty.Transparnt(img, opacity);
                                                gr1.DrawImage(img, rec);
                                                sp0.RemoveAt(i);
                                                i--;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ef)
                                {
                                    throw ef;
                                }
                            }
                            sp0.Clear();
                            spl4.Clear();
                            gr1.Dispose();
                            //workingBitmap1.Save(Path.Combine(UsrImageProc.SavedPhotos_Path, Path.GetFileName(inputBitmapPath)));
                            //imglist.Add("lokale kaydettim");
                            imglist.Add(ImageProperty.ImageToBase64(btm, System.Drawing.Imaging.ImageFormat.Jpeg));
                            btm.Dispose();
                        }
                    }
                    else
                    {
                        throw new Exception("Bu Formata Uygun Resimler Bulunamadı2") { Source = "" };
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return imglist.First();
        }
    }
}