using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Muuzy.Class
{
    public enum PixFormat
    {
        _null = 0,
        _12x12 = 12,
        _20x20 = 20,
        _36x36 = 36,
        _48x48 = 48,
        _64x64 = 64,
        _94x94 = 94
    }

    public class ImgSquare
    {
        public bool isArea = false;

        public int IAvgRgb { get; set; }

        private Image myVar;

        public Image IImage
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public ImgSquare(Image _img)
        {
            IImage = _img;
            //QuardAvg = avggs;
            QuardAvg = ImageProperty.Quard((Bitmap)IImage);
            string hex = Color.FromArgb(QuardAvg[4].QuardAvg).Name.Substring(2);
            IAvgRgb = ImageProperty.HexToInt(hex);
            isArea = false;
        }

        public int GeneratedColorCode
        {
            get
            {
                if (isArea)
                {
                    return SAvgArb;
                }
                else
                {
                    return IAvgRgb;
                }
            }
        }

        //120-360 r=>60<=g
        //120-240 g=>180<=b
        //240-360 g=>300<=b

        //r      r:255	 g:0		 b:0		 HSB:1			Saturation:360
        //r-g    r:255	 g:255		 b:0		 HSB:1			Saturation:60
        //g 	 r:0	 g:255		 b:0		 HSB:1			Saturation:120
        //b-g	 r:0	 g:255		 b:255		 HSB:1			Saturation:180
        //b 	 r:0	 g:0		 b:255		 HSB:1			Saturation:240
        //r-b	 r:255	 g:0		 b:255		 HSB:1			Saturation:300
        //r      r:255	 g:0		 b:0		 HSB:1			Saturation:360

        //baskın olan 2 Renk renk tonunu belirtiyor 3. renk de parlaklığı belirtiyor
        //2 Renk birbirine eşitse 3. renk ana rengi diğer 2 renkde parlaklığı belirtir

        //public int ComparePixColor(Color miniClr)
        //{
        //    Color a = Color.FromArgb(this.QuardAvg[QuardAvg.Count - 1].QAvgAbs);
        //    return 100 * (int)(
        //        1.0 - ((double)(
        //            Math.Abs(a.R - miniClr.R) +
        //            Math.Abs(a.G - miniClr.G) +
        //            Math.Abs(a.B - miniClr.B)
        //        ) / (256.0 * 3))
        //    );
        //}

        private string hexname = "";

        public int SAvgArb { get; set; }

        public int W { get; set; }

        public int H { get; set; }

        public int Brightness { get { return this.QuardAvg[QuardAvg.Count - 1].Brightness; } }

        public List<QuardPixAvg> QuardAvg { get; set; }

        public ImgSquare(int _w, int _h, List<QuardPixAvg> lquard)
        {
            W = _w;
            H = _h;
            hexname = Color.FromArgb(lquard[lquard.Count - 1].QuardAvg).Name;
            string hex = "";
            if (hexname.Length == 8)
                hex = hexname.Substring(2);
            else if (hexname.Length == 6)
                hex = hexname;
            else if (hexname.Length == 5)
                hex = "0" + hexname;
            else if (hexname.Length == 4)
                hex = "000" + hexname;
            else
                hex = "0";

            SAvgArb = ImageProperty.HexToInt(hex);
            QuardAvg = lquard;
            isArea = true;
        }

        public override string ToString()
        {
            return W + "x" + H;
            if (isArea)
            {
                return SAvgArb.ToString();
            }
            else
            {
                return IAvgRgb.ToString();
            }
        }

        ~ImgSquare()
        {
            hexname = "";
            SAvgArb = 0;
            QuardAvg = null;
            W = 0;
            H = 0;
            myVar = null;
            isArea = false;
            IAvgRgb = 0;
        }
    }

    public class QuardPixAvg
    {
        public QuardPixAvg()
        {
        }

        public QuardBolum Bolum { get; set; }

        public int QuardAvg { get; set; }

        public int QAvgAbs { get { return Math.Abs(QuardAvg); } }

        public int Brightness { get { return ImageProperty.BrightnessPercent(this.QuardAvg); } }

        public QuardPixAvg(Color cl, QuardBolum blm)
        {
            QuardAvg = cl.ToArgb();
            Bolum = blm;
        }

        public override string ToString()
        {
            return QuardAvg.ToString();
        }

        ~QuardPixAvg()
        {
            Bolum = new QuardBolum();
            QuardAvg = 0;
        }
    }

    public enum QuardBolum
    {
        SolUst = 0,
        SagUst = 1,
        SolAlt = 2,
        SagAlt = 3,
        TotalAvg = 4
    }

    public class ColorMatrix
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ColorMatrix()
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Matrix containing the values of the ColorMatrix
        /// </summary>
        public float[][] Matrix { get; set; }

        #endregion Properties

        #region Public Functions

        /// <summary>
        /// Applies the color matrix
        /// </summary>
        /// <param name="OriginalImage">Image sent in</param>
        /// <returns>An image with the color matrix applied</returns>
        public Bitmap Apply(Bitmap OriginalImage)
        {
            Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
            using (Graphics NewGraphics = Graphics.FromImage(NewBitmap))
            {
                System.Drawing.Imaging.ColorMatrix NewColorMatrix = new System.Drawing.Imaging.ColorMatrix(Matrix);
                using (ImageAttributes Attributes = new ImageAttributes())
                {
                    Attributes.SetColorMatrix(NewColorMatrix);
                    NewGraphics.DrawImage(OriginalImage,
                        new System.Drawing.Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height),
                       0, 0, OriginalImage.Width, OriginalImage.Height,
                        GraphicsUnit.Pixel,
                       Attributes);
                }
            }
            return NewBitmap;
        }

        #endregion Public Functions
    }
}