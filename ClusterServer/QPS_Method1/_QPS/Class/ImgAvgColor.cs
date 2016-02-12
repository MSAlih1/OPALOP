using QPS_Web1._CSHARP.Class;
using System.Drawing;

namespace QPS_Web1._QPS.Class
{
    public class ImageAvgColor
    {
        public int IAvgRgb { get; set; }

        private Image myVar;

        public Image IImage
        {
            get
            {
                return myVar;
            }
            set { myVar = value; }
        }

        public ImageAvgColor(Image _img)
        {
            IImage = _img;
            IAvgRgb = ImageProperty.GetImageAVGRgb((Bitmap)IImage);
        }
    }
}