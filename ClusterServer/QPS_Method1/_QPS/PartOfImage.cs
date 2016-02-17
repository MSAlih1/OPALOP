//using ImageMagick;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Web;

//namespace QPS_Method1._QPS
//{
    //public class PartOfImage
    //{
    //    public Rectangle ImagePartInfo { get; set; }
    //    public FileInfo ImageUrl { get; set; }

    //    private Bitmap newGeneratedImg = null;

    //    public Bitmap Image
    //    {
    //        get
    //        {
    //            if (ImageUrl != null)
    //            {
    //                MagickImage img = new MagickImage(ImageUrl);
    //                return img.ToBitmap();
    //            }
    //            else
    //            {
    //                return newGeneratedImg;
    //            }
    //        }
    //    }

    //    public PartOfImage(Bitmap btm, Rectangle recti)
    //    {
    //        newGeneratedImg = btm;
    //        ImagePartInfo = recti;
    //    }

    //    public PartOfImage(string ImgUrl, Rectangle recti)
    //    {
    //        ImageUrl = new FileInfo(ImgUrl);
    //        ImagePartInfo = recti;
    //    }
    //}
//}