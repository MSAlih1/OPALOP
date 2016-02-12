using QPS_Web1._CSHARP.Class;
using QPS_Web1._QPS.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace QPS_Web1._QPS.Class
{
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
}