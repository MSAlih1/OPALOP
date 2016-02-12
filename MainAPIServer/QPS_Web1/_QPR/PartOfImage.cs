using ImageMagick;
using System.Drawing;
using System.IO;

namespace Api._QPR
{
    public class PartOfImage
    {
        public Rectangle ImagePartInfo { get; set; }

        public FileInfo ImageUrl { get; set; }

        private MagickImage newGeneratedImg = null;

        public MagickImage Image
        {
            get
            {
                if (ImageUrl != null)
                {
                    MagickImage img = new MagickImage(ImageUrl);
                    return img;
                }
                else
                {
                    return newGeneratedImg;
                }
            }
        }

        public PartOfImage(Bitmap btm, Rectangle recti)
        {
            newGeneratedImg = new MagickImage(btm);
            ImagePartInfo = recti;
        }

        public PartOfImage(string ImgUrl, Rectangle recti)
        {
            ImageUrl = new FileInfo(ImgUrl);
            ImagePartInfo = recti;
        }
    }
}