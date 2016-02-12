using ImageMagick;
using QPS_Method1._QPS;
using QPS_Method1._QPS.Type;
using QPS_Web1._QPS;
using QPS_Web1._QPS.Class;
using QPS_Web1._QPS.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace QPS_Web1._CSHARP.Class
{
    public static class ImageProperty
    {
        private const int bytesPerPixel = 4;

        public static object StringToEnum(Type t, string Value)
        {
            foreach (FieldInfo fi in t.GetFields())
                if (fi.Name == Value)
                    return fi.GetValue(null);

            throw new Exception(string.Format("Can't convert {0} to {1}", Value, t.ToString()));
        }

        public static float[] angle = new float[] { 90, 180, 270, 360 };
        public static string JPG { get { return ".jpg"; } }

        #region rotate image

        public static Bitmap SmartRotate(ImgSquare _AreaSQ, ImgSquare _MiniSQ)
        {
            List<QuardPixAvg> Alq = _AreaSQ.QuardAvg.OrderBy(p => p.QAvgAbs).ToList();
            List<QuardPixAvg> Mlq = _MiniSQ.QuardAvg.OrderBy(p => p.QAvgAbs).ToList();
            int tolerans = 0;
            QuardPixAvg snc = new QuardPixAvg();
            int cont = 0;
            do
            {
                tolerans += 1050903;
                snc = Mlq
                   .Where(p =>
                       p.QAvgAbs + tolerans > Alq[Alq.Count - 1].QAvgAbs
                       &&
                       p.QAvgAbs - tolerans < Alq[Alq.Count - 1].QAvgAbs)
                        .FirstOrDefault();
                cont++;
            }
            while (snc == null || cont > 10);

            if (snc == null)
                return _MiniSQ.IImage as Bitmap;

            if (snc.Bolum == QuardBolum.SolUst)
                return RotateImage(_MiniSQ.IImage, angle[1]);
            else if (snc.Bolum == QuardBolum.SagUst)
                return RotateImage(_MiniSQ.IImage, angle[0]);
            //else if (snc.Bolum == ImageProcess.QuardBolum.SolAlt)
            //return ImageStatics.RotateImage(_MiniSQ.IImage, angle[2]);
            else if ((snc.Bolum == QuardBolum.SagAlt) || (snc.Bolum == QuardBolum.SolAlt))
                return _MiniSQ.IImage as Bitmap;

            return _MiniSQ.IImage as Bitmap;
        }

        public static Bitmap RotateImage(System.Drawing.Image image, float angle)
        {
            if (image == null)
            {
                return null;
            }

            Size size = image.Size;
            if (size.Width < 1 || size.Height < 1)
            {
                return null;
            }

            Bitmap tempImage = new Bitmap(size.Width, size.Height);
            using (Graphics tempGraphics = Graphics.FromImage(tempImage))
            {
                PointF center = new PointF((float)size.Width / 2F, (float)size.Height / 2F);
                tempGraphics.TranslateTransform(center.X, center.Y, MatrixOrder.Prepend);
                tempGraphics.RotateTransform(angle != 180F ? angle : 182F/*at 180 exact angle the rotate make a small shift of image I don't know why!*/);
                tempGraphics.TranslateTransform(-center.X, -center.Y, MatrixOrder.Prepend);
                tempGraphics.DrawImage(image, new Point(0, 0));
            }
            for (int _h = 0; _h < image.Height; _h++)
            {
                tempImage.SetPixel(0, _h, tempImage.GetPixel(1, _h));
                tempImage.SetPixel(image.Width - 1, _h, tempImage.GetPixel(image.Width - 2, _h));
            }
            for (int _w = 0; _w < image.Width; _w++)
            {
                tempImage.SetPixel(_w, 0, tempImage.GetPixel(_w, 1));
                tempImage.SetPixel(_w, image.Width - 1, tempImage.GetPixel(_w, image.Width - 2));
            }
            return tempImage;
        }

        #endregion rotate image

        #region parlaklık tespiti için

        public static int map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static int BrightnessPercent(int fronRGB)
        {
            return BrightnessPercent(Color.FromArgb(fronRGB));
        }

        public static int BrightnessPercent(Color cl)
        {
            return map(map(Color.FromArgb(cl.R, cl.G, cl.B).ToArgb(), -16777216, -1, 0, 255), 0, 255, 0, 100);
        }

        #endregion parlaklık tespiti için

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize.Clone() as Image, size));
        }

        public static Image resizeImage2(Image imgToResize, Size size)
        {
            Bitmap btm = new Bitmap(imgToResize, size);
            Graphics gr = Graphics.FromImage(btm);
            gr.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
            gr.Dispose();
            return btm;
        }

        public static List<QuardPixAvg> Quard(System.Drawing.Bitmap img)
        {
            int _w = img.Width / 2;
            int _h = img.Height / 2;
            List<Bitmap> lst = new List<System.Drawing.Bitmap>();
            lst.Add(img.Clone(new Rectangle(0, 0, _w, _h), img.PixelFormat) as Bitmap);
            lst.Add(img.Clone(new Rectangle(_w, 0, _w, _h), img.PixelFormat) as Bitmap);
            lst.Add(img.Clone(new Rectangle(0, _h, _w, _h), img.PixelFormat) as Bitmap);
            lst.Add(img.Clone(new Rectangle(_w, _h, _w, _h), img.PixelFormat) as Bitmap);

            int[,] pixavg = new int[5, 3];
            for (int W = 0; W < _w; W++)
            {
                for (int H = 0; H < _h; H++)
                {
                    Color a = lst[0].GetPixel(W, H);
                    pixavg[0, 0] += a.R;
                    pixavg[0, 1] += a.G;
                    pixavg[0, 2] += a.B;

                    Color b = lst[1].GetPixel(W, H);
                    pixavg[1, 0] += b.R;
                    pixavg[1, 1] += b.G;
                    pixavg[1, 2] += b.B;

                    Color c = lst[2].GetPixel(W, H);
                    pixavg[2, 0] += c.R;
                    pixavg[2, 1] += c.G;
                    pixavg[2, 2] += c.B;

                    Color d = lst[3].GetPixel(W, H);
                    pixavg[3, 0] += d.R;
                    pixavg[3, 1] += d.G;
                    pixavg[3, 2] += d.B;

                    if (W == 0 && H == 0 || W == _w - 1 && H == _h - 1)
                    {
                        continue;
                    }
                    pixavg[4, 0] += a.R + b.R + c.R;
                    pixavg[4, 1] += a.G + b.G + c.G;
                    pixavg[4, 2] += a.B + b.B + c.B;
                }
            }
            List<QuardPixAvg> lavg = new List<QuardPixAvg>();
            int totalminiPix = (_w * _h);
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

            int totalPix = (img.Width) * (img.Height);
            pixavg[4, 0] /= totalPix;
            pixavg[4, 1] /= totalPix;
            pixavg[4, 2] /= totalPix;

            lavg.Add(new QuardPixAvg
                (Color.FromArgb((pixavg[0, 0]), (pixavg[0, 1]), (pixavg[0, 2])), QuardBolum.SolUst));

            lavg.Add(new QuardPixAvg
                (Color.FromArgb((pixavg[1, 0]), (pixavg[1, 1]), (pixavg[1, 2])), QuardBolum.SagUst));

            lavg.Add(new QuardPixAvg
                (Color.FromArgb((pixavg[2, 0]), (pixavg[2, 1]), (pixavg[2, 2])), QuardBolum.SolAlt));

            lavg.Add(new QuardPixAvg(Color.FromArgb((pixavg[3, 0]), (pixavg[3, 1]), (pixavg[3, 2])), QuardBolum.SagAlt));

            lavg.Add(new QuardPixAvg
                (Color.FromArgb((pixavg[4, 0]), (pixavg[4, 1]), (pixavg[4, 2])), QuardBolum.TotalAvg));

            return lavg;
        }

        public static int GetImageAVGRgb(System.Drawing.Bitmap _im)
        {
            int _w = _im.Width - 1;
            int _h = _im.Height - 1;
            int r = 0, g = 0, b = 0;
            for (int w = 1; w < _w; w++)
            {
                for (int h = 1; h < _h; h++)
                {
                    Color c = _im.GetPixel(w, h);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                }
            }
            r = r / (_w * _w);
            g = g / (_w * _w);
            b = b / (_w * _w);
            string hex = Color.FromArgb(r, g, b).Name.Substring(2);
            int sayi = HexToInt(hex);
            return sayi;
        }

        public static int HexToInt(string val)
        {
            int sayi = Convert.ToInt32(val, 16);
            return sayi;
        }

        public static Bitmap OpenImageFile(string _path)
        {
            Bitmap img;
            StreamReader reader = new StreamReader(_path);
            img = (Bitmap)System.Drawing.Image.FromStream(reader.BaseStream);
            reader.Close();
            return img;
        }

        public static string GenerateRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X").ToLower();
        }

        public static Random random = new Random();

        public static string ImageToBase64(System.Drawing.Bitmap image, System.Drawing.Imaging.ImageFormat format)
        {
            string base64String = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                ms.Close();
                string formt = "jpg";

                if (format == ImageFormat.Png)
                    formt = "png";
                else if (format == ImageFormat.Jpeg)
                    formt = "jpg";

                base64String = "data:image/" + formt + ";base64," + Convert.ToBase64String(imageBytes);
                imageBytes = new byte[0];
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return base64String;
        }

        public static Bitmap Transparnt(System.Drawing.Image img, int opacity)
        {
            Bitmap orginal = new Bitmap(img);
            Bitmap transparent = new Bitmap(img.Width, img.Height);
            transparent.MakeTransparent(Color.Black);
            for (int i = 0; i < img.Width; i++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color c = orginal.GetPixel(i, y);
                    if (c != Color.Transparent)
                    {
                        int a = (int)((float)c.A * ((float)opacity / 100));
                        int r = (int)((float)c.R * ((float)opacity / 100));
                        int g = (int)((float)c.G * ((float)opacity / 100));
                        int b = (int)((float)c.B * ((float)opacity / 100));
                        Color newC = Color.FromArgb(a, r, g, b);
                        transparent.SetPixel(i, y, newC);
                    }
                }
            }
            orginal.Dispose();
            return transparent;
        }

        public static Bitmap AdjustBrightness(this Bitmap Image, int Value)
        {
            float FinalValue = (float)Value / 255.0f;
            QPS_Method1._QPS.Class.ColorMatrix TempMatrix = new QPS_Method1._QPS.Class.ColorMatrix();
            TempMatrix.Matrix = new float[][]{
                     new float[] {1, 0, 0, 0, 0},
                      new float[] {0, 1, 0, 0, 0},
                       new float[] {0, 0, 1, 0, 0},
                       new float[] {0, 0, 0, 1, 0},
                       new float[] {FinalValue, FinalValue, FinalValue, 1, 1}
                   };
            return TempMatrix.Apply(Image);
        }

        public static Bitmap ArithmeticBlend(this Bitmap sourceBitmap, Bitmap blendBitmap, ColorCalculationType calculationType)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            BitmapData blendData = blendBitmap.LockBits(new Rectangle(0, 0,
                                    blendBitmap.Width, blendBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] blendBuffer = new byte[blendData.Stride * blendData.Height];
            Marshal.Copy(blendData.Scan0, blendBuffer, 0, blendBuffer.Length);
            blendBitmap.UnlockBits(blendData);

            for (int k = 0; (k + 4 < pixelBuffer.Length) &&
                            (k + 4 < blendBuffer.Length); k += 4)
            {
                pixelBuffer[k] = Calculate(pixelBuffer[k],
                                 blendBuffer[k], calculationType);

                pixelBuffer[k + 1] = Calculate(pixelBuffer[k + 1],
                                     blendBuffer[k + 1], calculationType);

                pixelBuffer[k + 2] = Calculate(pixelBuffer[k + 2],
                                     blendBuffer[k + 2], calculationType);
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Bitmap Contrast(this Bitmap sourceBitmap, int threshold)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                        sourceBitmap.Width, sourceBitmap.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
            double blue = 0;
            double green = 0;
            double red = 0;

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
                           contrastLevel) + 0.5) * 255.0;

                green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0;

                red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
                           contrastLevel) + 0.5) * 255.0;

                if (blue > 255)
                { blue = 255; }
                else if (blue < 0)
                { blue = 0; }

                if (green > 255)
                { green = 255; }
                else if (green < 0)
                { green = 0; }

                if (red > 255)
                { red = 255; }
                else if (red < 0)
                { red = 0; }

                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            return resultBitmap;
        }

        public static byte Calculate(byte color1, byte color2, ColorCalculationType calculationType)
        {
            byte resultValue = 0;
            int intResult = 0;

            if (calculationType == ColorCalculationType.Add)
                intResult = color1 + color2;
            else if (calculationType == ColorCalculationType.Average)
                intResult = (color1 + color2) / 2;
            else if (calculationType == ColorCalculationType.SubtractLeft)
                intResult = color1 - color2;
            else if (calculationType == ColorCalculationType.SubtractRight)
                intResult = color2 - color1;
            else if (calculationType == ColorCalculationType.Difference)
                intResult = Math.Abs(color1 - color2);
            else if (calculationType == ColorCalculationType.Multiply)
                intResult = (int)((color1 / 255.0 * color2 / 255.0) * 255.0);
            else if (calculationType == ColorCalculationType.Min)
                intResult = (color1 < color2 ? color1 : color2);
            else if (calculationType == ColorCalculationType.Max)
                intResult = (color1 > color2 ? color1 : color2);
            else if (calculationType == ColorCalculationType.Amplitude)
                intResult = (int)(Math.Sqrt(color1 * color1 + color2 * color2) / Math.Sqrt(2.0));

            if (intResult < 0)
                resultValue = 0;
            else if (intResult > 255)
                resultValue = 255;
            else
                resultValue = (byte)intResult;

            return resultValue;
        }

        public static NewImagePart PointGenerator(string UserName, byte[] ImagePart, int _x, int _y, int width, int height, int PxFormat)
        {
            try
            {
                Rectangle recti = new Rectangle(_x, _y, width, height);
                PixFormat _pixFor = (PixFormat)StringToEnum(typeof(PixFormat), Convert.ToString("_" + PxFormat.ToString() + "x" + PxFormat.ToString()));
                qprPath resources = new qprPath(UserName);
                if (_pixFor == PixFormat._null)
                    throw new Exception("Format null olamaz") { Source = "" };

                using (MagickImage imagem = new MagickImage(ImagePart))
                {
                    //############# ResizeImage #############
                    //int yuzde = 100;
                    imagem.Quality = 100;
                    //int _w = imagem.Width + (imagem.Width / 100) * yuzde;
                    //int _h = imagem.Height + (imagem.Height / 100) * yuzde;
                    //int new_W = _w - (_w % (int)_pixFor);
                    //int new_H = _h - (_h % (int)_pixFor);
                    //imagem.Resize(new_W, new_H);
                    imagem.Blur(5, 5);

                    List<ImgSquare> sp0 = new List<ImgSquare>();
                    if (true)//!File.Exists(path)
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
                        ////////////////////////////// xml generate ///////////////////////////////

                        #region xml oluşturma

                        //XmlDocument doc = new XmlDocument();
                        //XmlElement root = doc.CreateElement("SquarePoints");
                        //root.SetAttribute("Count", sp0.Count.ToString());
                        //root.SetAttribute("PixFormat", _pixFor.ToString());
                        //foreach (ImgSquare item in sp0)
                        //{
                        //    XmlElement child = doc.CreateElement("SquarePoint");
                        //    child.SetAttribute("WxH", item.ToString());
                        //    child.SetAttribute("ColorAVG", item.SAvgArb.ToString());
                        //    List<QuardPixAvg> lstQ = item.QuardAvg;
                        //    child.SetAttribute("SolUst", lstQ[0].QuardAvg.ToString());
                        //    child.SetAttribute("SagUst", lstQ[1].QuardAvg.ToString());
                        //    child.SetAttribute("SolAlt", lstQ[2].QuardAvg.ToString());
                        //    child.SetAttribute("SagAlt", lstQ[3].QuardAvg.ToString());
                        //    root.AppendChild(child);
                        //}
                        //doc.AppendChild(root);
                        //if (!Directory.Exists(finf.Directory.FullName))
                        //    Directory.CreateDirectory(finf.Directory.FullName);
                        //doc.Save(path);
                        //doc = null;

                        #endregion xml oluşturma
                    }
                    else
                    {
                        #region xml okuma

                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(path);
                        //XmlNodeList root1 = doc.GetElementsByTagName("SquarePoints");
                        //XmlNode root2 = root1.Item(0);
                        //foreach (XmlNode item in root2.ChildNodes)
                        //{
                        //    try
                        //    {
                        //        XmlAttribute at1 = item.Attributes[0];
                        //        string[] point = at1.InnerText.Split('x');
                        //        XmlAttribute at2 = item.Attributes[1];
                        //        string ColorAvg = at2.InnerText;
                        //        List<QuardPixAvg> lQuard = new List<QuardPixAvg>();
                        //        int SolUst = int.Parse(item.Attributes[2].InnerText);
                        //        int SagUst = int.Parse(item.Attributes[3].InnerText);
                        //        int SolAlt = int.Parse(item.Attributes[4].InnerText);
                        //        int SagAlt = int.Parse(item.Attributes[5].InnerText);
                        //        int Tumu = int.Parse(item.Attributes[1].InnerText);
                        //        lQuard.Add(new QuardPixAvg(Color.FromArgb(SolUst), QuardBolum.SolUst));
                        //        lQuard.Add(new QuardPixAvg(Color.FromArgb(SagUst), QuardBolum.SagUst));
                        //        lQuard.Add(new QuardPixAvg(Color.FromArgb(SolAlt), QuardBolum.SolAlt));
                        //        lQuard.Add(new QuardPixAvg(Color.FromArgb(SagAlt), QuardBolum.SagAlt));
                        //        lQuard.Add(new QuardPixAvg(Color.FromArgb(Tumu), QuardBolum.Tumu));
                        //        sp0.Add(new ImgSquare(int.Parse(point[0]), int.Parse(point[1]), lQuard));
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        throw e;
                        //    }
                        //}
                        //root2 = null;
                        //root1 = null;
                        //doc = null;

                        #endregion xml okuma
                    }
                    //#---------------------------------
                    int opacity = 50;
                    switch (_pixFor)
                    {
                        case PixFormat._null:
                            sp0.Clear();
                            throw new Exception("Tanımsız format") { Source = "" };
                            break;

                        case PixFormat._12x12: //12:36 sn
                            opacity = 50;
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
                    //string MiniPicturePath = Path.Combine(UserProperty.Data_Path, PixFormat._94x94.ToString());  //eski yükleme şekli
                    string MiniPicturePath = Path.Combine(resources.Data_InstagramPhotos);
                    if (Directory.Exists(MiniPicturePath))
                    {
                        Bitmap btm = imagem.ToBitmap();
                        using (Graphics gr1 = Graphics.FromImage(btm))
                        {
                            //string[] list = Directory.GetFiles(MiniPicturePath, "*" + JPG);
                            //string[] list = new string[0];
                            ///////////////////////////////////////////////////////////////
                            string[] file = Directory.GetFiles(resources.Current_User, resources.UserXmlInfo);

                            if (file.Count() != 1)
                                throw new Exception("Geçersiz kullanıcı bilgileri");

                            XDocument doc = XDocument.Load(file[0]);
                            XElement root = doc.Elements("_" + resources.UserName).First();
                            XElement InstagramP = root.Elements("InstagramPhotos").FirstOrDefault();
                            XElement[] photos = (from p in InstagramP.Elements() where p.Attribute("useThis").Value.ToLower() == "true" select p).ToArray();
                            //list = new string[photos.Count()];

                            //for (int i = 0; i < photos.Count(); i++)
                            //    list[i] = Path.Combine(MiniPicturePath, photos[i].Value);

                            ///////////////////////////////////////////////////
                            if (photos.Count() == 0)
                                throw new Exception("Bu Formata Uygun Resimler Bulunamadı") { Source = "" };

                            List<ImgSquare> spl2 = new List<ImgSquare>();
                            for (int i = 0; i < photos.Count(); i++)
                            {
                                if (File.Exists(Path.Combine(MiniPicturePath, Path.GetFileName(photos[i].Value))))
                                {
                                    using (MagickImage mini = new MagickImage(Path.Combine(MiniPicturePath, Path.GetFileName(photos[i].Value))))
                                    {
                                        mini.Quality = 100;
                                        if (mini.Width != 94 && mini.Height != 94)
                                            mini.Resize((int)_pixFor, (int)_pixFor);

                                        //ImgSquare imgsq = new ImgSquare(mini.ToBitmap(), new List<QuardPixAvg>() {
                                        //    new QuardPixAvg(Color.FromArgb(int.Parse(photos[i].Attribute("SolUst").Value)),QuardBolum.SagAlt),
                                        //    new QuardPixAvg(Color.FromArgb(int.Parse(photos[i].Attribute("SagUst").Value)),QuardBolum.SagAlt),
                                        //    new QuardPixAvg(Color.FromArgb(int.Parse(photos[i].Attribute("SolAlt").Value)),QuardBolum.SagAlt),
                                        //    new QuardPixAvg(Color.FromArgb(int.Parse(photos[i].Attribute("SagAlt").Value)),QuardBolum.SagAlt),
                                        //    new QuardPixAvg(Color.FromArgb(int.Parse(photos[i].Attribute("TotalAvg").Value)),QuardBolum.SagAlt)
                                        //});

                                        spl2.Add(new ImgSquare(mini.ToBitmap()));

                                        mini.Dispose();
                                    }
                                }
                            }
                            photos = null;
                            doc = null;
                            root = null;
                            InstagramP = null;
                            List<ImgSquare> spl4 = new List<ImgSquare>();
                            spl4.AddRange(sp0);
                            spl4.AddRange(spl2);
                            spl2.Clear();
                            spl4 = spl4.OrderBy(p => p.GeneratedColorCode).ToList();
                            int qpiro_number = 432101;
                            int qpiro_number2 = 0;
                            int undefined = 0;
                            for (int i = 0; i < sp0.Count; i++)
                            {
                                ImgSquare item = sp0[i];
                                //if (item.SAvgArb == 0 && item.IAvgRgb == 0)
                                //    continue;
                                try
                                {
                                    qpiro_number2 = 0;
                                    List<ImgSquare> snc = null;
                                    int cont = 0;
                                    do
                                    {
                                        snc = spl4.Where(p =>
                                            (p.GeneratedColorCode - (qpiro_number + qpiro_number2) < item.GeneratedColorCode &&
                                            p.GeneratedColorCode + (qpiro_number + qpiro_number2) > item.GeneratedColorCode) &&
                                            p.isArea == false).ToList();
                                        qpiro_number2 += 332101;
                                        cont++;
                                    }
                                    while (snc.Count == 0 && cont < 5);

                                    Rectangle rec = new Rectangle(item.W, item.H, (int)_pixFor, (int)_pixFor);

                                    if (snc.Count != 0)
                                    {
                                        int randi = random.Next(0, snc.Count);
                                        System.Drawing.Image img = SmartRotate(item, snc[randi]);
                                        snc.Clear();//
                                        img = Transparnt(img, opacity);
                                        gr1.DrawImage(img, rec);
                                        sp0.RemoveAt(i);
                                        i--;
                                        img.Dispose();
                                    }
                                    else
                                    {
                                        using (MagickImage imgUndefined = new MagickImage(Path.Combine(resources.Data_InstagramPhotos, "black.jpg")))
                                        {
                                            System.Drawing.Image img = Transparnt(imgUndefined.ToBitmap(), opacity);
                                            gr1.DrawImage(img, rec);
                                            sp0.RemoveAt(i);
                                            i--;
                                            imgUndefined.Dispose();
                                        }
                                        //int ss = item.GeneratedColorCode;
                                        //if (ss >= 11075440 && ss <= 15260100)
                                        //{
                                        //    cont = 0;
                                        // qpiro_number2 += 332101;
                                        //    do
                                        //    {
                                        //        snc = spl4.Where(p =>
                                        //            (p.GeneratedColorCode - (qpiro_number + qpiro_number2) < item.GeneratedColorCode &&
                                        //            p.GeneratedColorCode + (qpiro_number + qpiro_number2) > item.GeneratedColorCode) &&
                                        //            p.isArea == false).ToList();
                                        //        qpiro_number2 += 332101;
                                        //        cont++;
                                        //    }
                                        //    while (snc.Count == 0 && cont < 13);

                                        //    if (snc.Count != 0)
                                        //    {
                                        //        Rectangle rec = new Rectangle(item.W, item.H, (int)_pixFor, (int)_pixFor);
                                        //        int randi = random.Next(0, snc.Count);
                                        //        System.Drawing.Image img = SmartRotate(item, snc[randi]);
                                        //        snc.Clear();//
                                        //        img = Transparnt(img, opacity);
                                        //        gr1.DrawImage(img, rec);
                                        //        sp0.RemoveAt(i);
                                        //        i--;
                                        //    }
                                        //    else
                                        //    {
                                        //    }
                                        //}
                                        undefined++;
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
                            ////workingBitmap1.Save(Path.Combine(UsrImageProc.SavedPhotos_Path, Path.GetFileName(inputBitmapPath)));
                            ////imglist.Add("lokale kaydettim");
                            //Bitmap hd = btm.ArithmeticBlend(btm, ColorCalculationType.Add);
                            //imglist.Add(ImageToBase64(hd, System.Drawing.Imaging.ImageFormat.Jpeg));
                            //imglist.Add(ImageToBase64(btm, System.Drawing.Imaging.ImageFormat.Jpeg));
                            //btm.Dispose();
                            //hd.Dispose();
                            imagem.Dispose();
                            return new NewImagePart() { newImage = new MagickImage(btm).ToByteArray(), ImagePartInfo = recti.ToString() };
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
            return null;
        }
    }
}