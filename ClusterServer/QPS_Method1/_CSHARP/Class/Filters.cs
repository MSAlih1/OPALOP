namespace QPS_Web1._CSHARP.Class
{
    //public static class Filters
    //{
    //    static int[] HistogramRed = new int[256];
    //    static int[] HistogramGreen = new int[256];
    //    static int[] HistogramBlue = new int[256];
    //    static int[] HistogramRedK = new int[256];
    //    static int[] HistogramGreenK = new int[256];
    //    static int[] HistogramBlueK = new int[256];
    //    static int[] YüzdelikRed = new int[256];
    //    static int[] YüzdelikGreen = new int[256];
    //    static int[] YüzdelikBlue = new int[256];

    //    static String a = "";

    //    public static Bitmap HistogramEqualization(Bitmap _kaynakResim)
    //    {
    //        Bitmap renderedImage = _kaynakResim;

    //        BitmapData sourceData = renderedImage.LockBits(new Rectangle(0, 0,
    //                                    renderedImage.Width, renderedImage.Height),
    //                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

    //        byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
    //        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
    //        renderedImage.UnlockBits(sourceData);

    //        uint pixels = (uint)renderedImage.Height * (uint)renderedImage.Width;
    //        decimal Const = 255 / (decimal)pixels;

    //        int x, y, R, G, B;

    //        int[] HistogramRed2 = new int[256];
    //        int[] HistogramGreen2 = new int[256];
    //        int[] HistogramBlue2 = new int[256];

    //        for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
    //        {
    //            double blue = pixelBuffer[k];
    //            double green = pixelBuffer[k + 1];
    //            double red = pixelBuffer[k + 2];

    //            HistogramRed2[(int)red]++;
    //            HistogramGreen2[(int)green]++;
    //            HistogramBlue2[(int)blue]++;
    //        }

    //        int[] cdfR = HistogramRed2;
    //        int[] cdfG = HistogramGreen2;
    //        int[] cdfB = HistogramBlue2;

    //        for (int r = 1; r <= 255; r++)
    //        {
    //            cdfR[r] = cdfR[r] + cdfR[r - 1];
    //            cdfG[r] = cdfG[r] + cdfG[r - 1];
    //            cdfB[r] = cdfB[r] + cdfB[r - 1];
    //        }

    //        for (y = 0; y < renderedImage.Height; y++)
    //        {
    //            for (x = 0; x < renderedImage.Width; x++)
    //            {
    //                Color pixelColor = renderedImage.GetPixel(x, y);

    //                R = (int)((decimal)cdfR[pixelColor.R] * Const);
    //                G = (int)((decimal)cdfG[pixelColor.G] * Const);
    //                B = (int)((decimal)cdfB[pixelColor.B] * Const);

    //                Color newColor = Color.FromArgb(R, G, B);
    //                renderedImage.SetPixel(x, y, newColor);
    //            }
    //        }
    //        return renderedImage;
    //    }

    //    public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
    //    {
    //        float ratio = 1.0f;
    //        int maxSide = sourceBitmap.Width > sourceBitmap.Height ?
    //                      sourceBitmap.Width : sourceBitmap.Height;

    //        ratio = (float)maxSide / (float)canvasWidthLenght;

    //        Bitmap bitmapResult = (sourceBitmap.Width > sourceBitmap.Height ?
    //                                new Bitmap(canvasWidthLenght, (int)(sourceBitmap.Height / ratio))
    //                                : new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLenght));

    //        using (Graphics graphicsResult = Graphics.FromImage(bitmapResult))
    //        {
    //            graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
    //            graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //            graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

    //            graphicsResult.DrawImage(sourceBitmap,
    //                                    new Rectangle(0, 0,
    //                                        bitmapResult.Width, bitmapResult.Height),
    //                                    new Rectangle(0, 0,
    //                                        sourceBitmap.Width, sourceBitmap.Height),
    //                                        GraphicsUnit.Pixel);
    //            graphicsResult.Flush();
    //        }

    //        return bitmapResult;
    //    }

    //    //Contrast(Value)  +-100
    //    public static Bitmap Contrast(this Bitmap sourceBitmap, int threshold)
    //    {
    //        BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
    //                                    sourceBitmap.Width, sourceBitmap.Height),
    //                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

    //        byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

    //        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

    //        sourceBitmap.UnlockBits(sourceData);

    //        double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);

    //        double blue = 0;
    //        double green = 0;
    //        double red = 0;

    //        for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
    //        {
    //            blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
    //                       contrastLevel) + 0.5) * 255.0;

    //            green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
    //                        contrastLevel) + 0.5) * 255.0;

    //            red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
    //                       contrastLevel) + 0.5) * 255.0;

    //            if (blue > 255)
    //            { blue = 255; }
    //            else if (blue < 0)
    //            { blue = 0; }

    //            if (green > 255)
    //            { green = 255; }
    //            else if (green < 0)
    //            { green = 0; }

    //            if (red > 255)
    //            { red = 255; }
    //            else if (red < 0)
    //            { red = 0; }

    //            pixelBuffer[k] = (byte)blue;
    //            pixelBuffer[k + 1] = (byte)green;
    //            pixelBuffer[k + 2] = (byte)red;
    //        }
    //        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
    //        BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
    //                                    resultBitmap.Width, resultBitmap.Height),
    //                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

    //        Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
    //        resultBitmap.UnlockBits(resultData);
    //        return resultBitmap;
    //    }

    //    //ImageBlurFilters
    //    public static Bitmap ImageBlurFilter(this Bitmap sourceBitmap,
    //                                          BlurType blurType)
    //    {
    //        Bitmap resultBitmap = null;

    //        switch (blurType)
    //        {
    //            case BlurType.Mean3x3:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                                   Matrix.Mean3x3, 1.0 / 9.0, 0);
    //                } break;
    //            case BlurType.Mean5x5:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                                   Matrix.Mean5x5, 1.0 / 25.0, 0);
    //                } break;
    //            case BlurType.Mean7x7:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                                   Matrix.Mean7x7, 1.0 / 49.0, 0);
    //                } break;
    //            case BlurType.Mean9x9:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                                   Matrix.Mean9x9, 1.0 / 81.0, 0);
    //                } break;
    //            case BlurType.GaussianBlur3x3:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                            Matrix.GaussianBlur3x3, 1.0 / 16.0, 0);
    //                } break;
    //            case BlurType.GaussianBlur5x5:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                           Matrix.GaussianBlur5x5, 1.0 / 159.0, 0);
    //                } break;
    //            case BlurType.MotionBlur5x5:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                               Matrix.MotionBlur5x5, 1.0 / 10.0, 0);
    //                } break;
    //            case BlurType.MotionBlur5x5At45Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur5x5At45Degrees, 1.0 / 5.0, 0);
    //                } break;
    //            case BlurType.MotionBlur5x5At135Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur5x5At135Degrees, 1.0 / 5.0, 0);
    //                } break;
    //            case BlurType.MotionBlur7x7:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur7x7, 1.0 / 14.0, 0);
    //                } break;
    //            case BlurType.MotionBlur7x7At45Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur7x7At45Degrees, 1.0 / 7.0, 0);
    //                } break;
    //            case BlurType.MotionBlur7x7At135Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur7x7At135Degrees, 1.0 / 7.0, 0);
    //                } break;
    //            case BlurType.MotionBlur9x9:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur9x9, 1.0 / 18.0, 0);
    //                } break;
    //            case BlurType.MotionBlur9x9At45Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur9x9At45Degrees, 1.0 / 9.0, 0);
    //                } break;
    //            case BlurType.MotionBlur9x9At135Degrees:
    //                {
    //                    resultBitmap = sourceBitmap.ConvolutionFilter(
    //                    Matrix.MotionBlur9x9At135Degrees, 1.0 / 9.0, 0);
    //                } break;
    //            case BlurType.Median3x3:
    //                {
    //                    resultBitmap = sourceBitmap.MedianFilter(3);
    //                } break;
    //            case BlurType.Median5x5:
    //                {
    //                    resultBitmap = sourceBitmap.MedianFilter(5);
    //                } break;
    //            case BlurType.Median7x7:
    //                {
    //                    resultBitmap = sourceBitmap.MedianFilter(7);
    //                } break;
    //            case BlurType.Median9x9:
    //                {
    //                    resultBitmap = sourceBitmap.MedianFilter(9);
    //                } break;
    //            case BlurType.Median11x11:
    //                {
    //                    resultBitmap = sourceBitmap.MedianFilter(11);
    //                } break;
    //        }

    //        return resultBitmap;
    //    }

    //    public enum BlurType
    //    {
    //        Mean3x3,
    //        Mean5x5,
    //        Mean7x7,
    //        Mean9x9,
    //        GaussianBlur3x3,
    //        GaussianBlur5x5,
    //        MotionBlur5x5,
    //        MotionBlur5x5At45Degrees,
    //        MotionBlur5x5At135Degrees,
    //        MotionBlur7x7,
    //        MotionBlur7x7At45Degrees,
    //        MotionBlur7x7At135Degrees,
    //        MotionBlur9x9,
    //        MotionBlur9x9At45Degrees,
    //        MotionBlur9x9At135Degrees,
    //        Median3x3,
    //        Median5x5,
    //        Median7x7,
    //        Median9x9,
    //        Median11x11
    //    }

    //    public static class Matrix
    //    {
    //        public static double[,] Mean3x3
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 1, 1, },
    //              { 1, 1, 1, },
    //              { 1, 1, 1, }, };
    //            }
    //        }

    //        public static double[,] Mean5x5
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1}, };
    //            }
    //        }

    //        public static double[,] Mean7x7
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1}, };
    //            }
    //        }

    //        public static double[,] Mean9x9
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1},
    //              { 1, 1, 1, 1, 1, 1, 1, 1, 1}, };
    //            }
    //        }

    //        public static double[,] GaussianBlur3x3
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 2, 1, },
    //              { 2, 4, 2, },
    //              { 1, 2, 1, }, };
    //            }
    //        }

    //        public static double[,] GaussianBlur5x5
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 2, 04, 05, 04, 2 },
    //              { 4, 09, 12, 09, 4 },
    //              { 5, 12, 15, 12, 5 },
    //              { 4, 09, 12, 09, 4 },
    //              { 2, 04, 05, 04, 2 }, };
    //            }
    //        }

    //        public static double[,] MotionBlur5x5
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 0, 0, 0, 1},
    //              { 0, 1, 0, 1, 0},
    //              { 0, 0, 1, 0, 0},
    //              { 0, 1, 0, 1, 0},
    //              { 1, 0, 0, 0, 1}, };
    //            }
    //        }

    //        public static double[,] MotionBlur5x5At45Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 0, 0, 0, 0, 1},
    //              { 0, 0, 0, 1, 0},
    //              { 0, 0, 1, 0, 0},
    //              { 0, 1, 0, 0, 0},
    //              { 1, 0, 0, 0, 0}, };
    //            }
    //        }

    //        public static double[,] MotionBlur5x5At135Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 0, 0, 0, 0},
    //              { 0, 1, 0, 0, 0},
    //              { 0, 0, 1, 0, 0},
    //              { 0, 0, 0, 1, 0},
    //              { 0, 0, 0, 0, 1}, };
    //            }
    //        }

    //        public static double[,] MotionBlur7x7
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 0, 0, 0, 0, 0, 1},
    //              { 0, 1, 0, 0, 0, 1, 0},
    //              { 0, 0, 1, 0, 1, 0, 0},
    //              { 0, 0, 0, 1, 0, 0, 0},
    //              { 0, 0, 1, 0, 1, 0, 0},
    //              { 0, 1, 0, 0, 0, 1, 0},
    //              { 1, 0, 0, 0, 0, 0, 1}, };
    //            }
    //        }

    //        public static double[,] MotionBlur7x7At45Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 0, 0, 0, 0, 0, 0, 1},
    //              { 0, 0, 0, 0, 0, 1, 0},
    //              { 0, 0, 0, 0, 1, 0, 0},
    //              { 0, 0, 0, 1, 0, 0, 0},
    //              { 0, 0, 1, 0, 0, 0, 0},
    //              { 0, 1, 0, 0, 0, 0, 0},
    //              { 1, 0, 0, 0, 0, 0, 0}, };
    //            }
    //        }

    //        public static double[,] MotionBlur7x7At135Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { { 1, 0, 0, 0, 0, 0, 0},
    //              { 0, 1, 0, 0, 0, 0, 0},
    //              { 0, 0, 1, 0, 0, 0, 0},
    //              { 0, 0, 0, 1, 0, 0, 0},
    //              { 0, 0, 0, 0, 1, 0, 0},
    //              { 0, 0, 0, 0, 0, 1, 0},
    //              { 0, 0, 0, 0, 0, 0, 1}, };
    //            }
    //        }

    //        public static double[,] MotionBlur9x9
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { {1, 0, 0, 0, 0, 0, 0, 0, 1,},
    //              {0, 1, 0, 0, 0, 0, 0, 1, 0,},
    //              {0, 0, 1, 0, 0, 0, 1, 0, 0,},
    //              {0, 0, 0, 1, 0, 1, 0, 0, 0,},
    //              {0, 0, 0, 0, 1, 0, 0, 0, 0,},
    //              {0, 0, 0, 1, 0, 1, 0, 0, 0,},
    //              {0, 0, 1, 0, 0, 0, 1, 0, 0,},
    //              {0, 1, 0, 0, 0, 0, 0, 1, 0,},
    //              {1, 0, 0, 0, 0, 0, 0, 0, 1,}, };
    //            }
    //        }

    //        public static double[,] MotionBlur9x9At45Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { {0, 0, 0, 0, 0, 0, 0, 0, 1,},
    //              {0, 0, 0, 0, 0, 0, 0, 1, 0,},
    //              {0, 0, 0, 0, 0, 0, 1, 0, 0,},
    //              {0, 0, 0, 0, 0, 1, 0, 0, 0,},
    //              {0, 0, 0, 0, 1, 0, 0, 0, 0,},
    //              {0, 0, 0, 1, 0, 0, 0, 0, 0,},
    //              {0, 0, 1, 0, 0, 0, 0, 0, 0,},
    //              {0, 1, 0, 0, 0, 0, 0, 0, 0,},
    //              {1, 0, 0, 0, 0, 0, 0, 0, 0,}, };
    //            }
    //        }

    //        public static double[,] MotionBlur9x9At135Degrees
    //        {
    //            get
    //            {
    //                return new double[,]
    //            { {1, 0, 0, 0, 0, 0, 0, 0, 0,},
    //              {0, 1, 0, 0, 0, 0, 0, 0, 0,},
    //              {0, 0, 1, 0, 0, 0, 0, 0, 0,},
    //              {0, 0, 0, 1, 0, 0, 0, 0, 0,},
    //              {0, 0, 0, 0, 1, 0, 0, 0, 0,},
    //              {0, 0, 0, 0, 0, 1, 0, 0, 0,},
    //              {0, 0, 0, 0, 0, 0, 1, 0, 0,},
    //              {0, 0, 0, 0, 0, 0, 0, 1, 0,},
    //              {0, 0, 0, 0, 0, 0, 0, 0, 1,}, };
    //            }
    //        }
    //    }

    //    private static Bitmap ConvolutionFilter(this Bitmap sourceBitmap,
    //                                              double[,] filterMatrix,
    //                                                   double factor = 1,
    //                                                        int bias = 0)
    //    {
    //        BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
    //                                 sourceBitmap.Width, sourceBitmap.Height),
    //                                                   ImageLockMode.ReadOnly,
    //                                             PixelFormat.Format32bppArgb);

    //        byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
    //        byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

    //        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
    //        sourceBitmap.UnlockBits(sourceData);

    //        double blue = 0.0;
    //        double green = 0.0;
    //        double red = 0.0;

    //        int filterWidth = filterMatrix.GetLength(1);
    //        int filterHeight = filterMatrix.GetLength(0);

    //        int filterOffset = (filterWidth - 1) / 2;
    //        int calcOffset = 0;

    //        int byteOffset = 0;

    //        for (int offsetY = filterOffset; offsetY <
    //            sourceBitmap.Height - filterOffset; offsetY++)
    //        {
    //            for (int offsetX = filterOffset; offsetX <
    //                sourceBitmap.Width - filterOffset; offsetX++)
    //            {
    //                blue = 0;
    //                green = 0;
    //                red = 0;

    //                byteOffset = offsetY *
    //                             sourceData.Stride +
    //                             offsetX * 4;

    //                for (int filterY = -filterOffset;
    //                    filterY <= filterOffset; filterY++)
    //                {
    //                    for (int filterX = -filterOffset;
    //                        filterX <= filterOffset; filterX++)
    //                    {
    //                        calcOffset = byteOffset +
    //                                     (filterX * 4) +
    //                                     (filterY * sourceData.Stride);

    //                        blue += (double)(pixelBuffer[calcOffset]) *
    //                                filterMatrix[filterY + filterOffset,
    //                                                    filterX + filterOffset];

    //                        green += (double)(pixelBuffer[calcOffset + 1]) *
    //                                 filterMatrix[filterY + filterOffset,
    //                                                    filterX + filterOffset];

    //                        red += (double)(pixelBuffer[calcOffset + 2]) *
    //                               filterMatrix[filterY + filterOffset,
    //                                                  filterX + filterOffset];
    //                    }
    //                }

    //                blue = factor * blue + bias;
    //                green = factor * green + bias;
    //                red = factor * red + bias;

    //                blue = (blue > 255 ? 255 :
    //                       (blue < 0 ? 0 :
    //                        blue));

    //                green = (green > 255 ? 255 :
    //                        (green < 0 ? 0 :
    //                         green));

    //                red = (red > 255 ? 255 :
    //                      (red < 0 ? 0 :
    //                       red));

    //                resultBuffer[byteOffset] = (byte)(blue);
    //                resultBuffer[byteOffset + 1] = (byte)(green);
    //                resultBuffer[byteOffset + 2] = (byte)(red);
    //                resultBuffer[byteOffset + 3] = 255;
    //            }
    //        }

    //        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

    //        BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
    //                                 resultBitmap.Width, resultBitmap.Height),
    //                                                  ImageLockMode.WriteOnly,
    //                                             PixelFormat.Format32bppArgb);

    //        Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
    //        resultBitmap.UnlockBits(resultData);

    //        return resultBitmap;
    //    }

    //    public static Bitmap MedianFilter(this Bitmap sourceBitmap,
    //                                                int matrixSize)
    //    {
    //        BitmapData sourceData =
    //                   sourceBitmap.LockBits(new Rectangle(0, 0,
    //                   sourceBitmap.Width, sourceBitmap.Height),
    //                   ImageLockMode.ReadOnly,
    //                   PixelFormat.Format32bppArgb);

    //        byte[] pixelBuffer = new byte[sourceData.Stride *
    //                                      sourceData.Height];

    //        byte[] resultBuffer = new byte[sourceData.Stride *
    //                                       sourceData.Height];

    //        Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
    //                                   pixelBuffer.Length);

    //        sourceBitmap.UnlockBits(sourceData);

    //        int filterOffset = (matrixSize - 1) / 2;
    //        int calcOffset = 0;

    //        int byteOffset = 0;

    //        List<int> neighbourPixels = new List<int>();
    //        byte[] middlePixel;

    //        for (int offsetY = filterOffset; offsetY <
    //            sourceBitmap.Height - filterOffset; offsetY++)
    //        {
    //            for (int offsetX = filterOffset; offsetX <
    //                sourceBitmap.Width - filterOffset; offsetX++)
    //            {
    //                byteOffset = offsetY *
    //                             sourceData.Stride +
    //                             offsetX * 4;

    //                neighbourPixels.Clear();

    //                for (int filterY = -filterOffset;
    //                    filterY <= filterOffset; filterY++)
    //                {
    //                    for (int filterX = -filterOffset;
    //                        filterX <= filterOffset; filterX++)
    //                    {
    //                        calcOffset = byteOffset +
    //                                     (filterX * 4) +
    //                                     (filterY * sourceData.Stride);

    //                        neighbourPixels.Add(BitConverter.ToInt32(
    //                                         pixelBuffer, calcOffset));
    //                    }
    //                }

    //                neighbourPixels.Sort();

    //                middlePixel = BitConverter.GetBytes(
    //                                   neighbourPixels[filterOffset]);

    //                resultBuffer[byteOffset] = middlePixel[0];
    //                resultBuffer[byteOffset + 1] = middlePixel[1];
    //                resultBuffer[byteOffset + 2] = middlePixel[2];
    //                resultBuffer[byteOffset + 3] = middlePixel[3];
    //            }
    //        }

    //        Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
    //                                         sourceBitmap.Height);

    //        BitmapData resultData =
    //                   resultBitmap.LockBits(new Rectangle(0, 0,
    //                   resultBitmap.Width, resultBitmap.Height),
    //                   ImageLockMode.WriteOnly,
    //                   PixelFormat.Format32bppArgb);

    //        Marshal.Copy(resultBuffer, 0, resultData.Scan0,
    //                                   resultBuffer.Length);

    //        resultBitmap.UnlockBits(resultData);

    //        return resultBitmap;
    //    }
    //    ///Adjusts the brightness
    //    ///</summary>
    //    ///<param name="Image">Image to change</param>
    //    ///<param name="Value">-255 to 255</param>
    //    ///<returns>A bitmap object</returns>
    //    public static Bitmap AdjustBrightness(Bitmap Image, int Value)
    //    {
    //        float FinalValue = (float)Value / 255.0f;
    //        ColorMatrix TempMatrix = new ColorMatrix();
    //        TempMatrix.Matrix = new float[][]{
    //                  new float[] {1, 0, 0, 0, 0},
    //                  new float[] {0, 1, 0, 0, 0},
    //                  new float[] {0, 0, 1, 0, 0},
    //                  new float[] {0, 0, 0, 1, 0},
    //                  new float[] {FinalValue, FinalValue, FinalValue, 1, 1}
    //              };
    //        return TempMatrix.Apply(Image);
    //    }
    //    /// <summary>
    //    /// Helper class for setting up and applying a color matrix
    //    /// </summary>
    //    public class ColorMatrix
    //    {
    //        #region Constructor

    //        /// <summary>
    //        /// Constructor
    //        /// </summary>
    //        public ColorMatrix()
    //        {
    //        }

    //        #endregion

    //        #region Properties

    //        /// <summary>
    //        /// Matrix containing the values of the ColorMatrix
    //        /// </summary>
    //        public float[][] Matrix { get; set; }

    //        #endregion

    //        #region Public Functions

    //        /// <summary>
    //        /// Applies the color matrix
    //        /// </summary>
    //        /// <param name="OriginalImage">Image sent in</param>
    //        /// <returns>An image with the color matrix applied</returns>
    //        public Bitmap Apply(Bitmap OriginalImage)
    //        {
    //            Bitmap NewBitmap = new Bitmap(OriginalImage.Width, OriginalImage.Height);
    //            using (Graphics NewGraphics = Graphics.FromImage(NewBitmap))
    //            {
    //                System.Drawing.Imaging.ColorMatrix NewColorMatrix = new System.Drawing.Imaging.ColorMatrix(Matrix);
    //                using (ImageAttributes Attributes = new ImageAttributes())
    //                {
    //                    Attributes.SetColorMatrix(NewColorMatrix);
    //                    NewGraphics.DrawImage(OriginalImage,
    //                        new System.Drawing.Rectangle(0, 0, OriginalImage.Width, OriginalImage.Height),
    //                        0, 0, OriginalImage.Width, OriginalImage.Height,
    //                        GraphicsUnit.Pixel,
    //                        Attributes);
    //                }
    //            }
    //            return NewBitmap;
    //        }

    //        #endregion
    //    }
    //}
}