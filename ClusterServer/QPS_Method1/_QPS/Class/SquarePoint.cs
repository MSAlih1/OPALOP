using QPS_Web1._CSHARP.Class;

namespace QPS_Web1._QPS.Class
{
    public class SquarePoint
    {
        private string hexname = "";

        public int SAvgArb { get; set; }

        public int W { get; set; }

        public int H { get; set; }

        public SquarePoint(int _w, int _h, string _colornexname)
        {
            W = _w;
            H = _h;
            hexname = _colornexname;
            string hex = "";
            if (hexname.Length == 8)
            {
                hex = hexname.Substring(2);
            }
            else if (hexname.Length == 6)
            {
                hex = hexname;
            }
            else if (hexname.Length == 5)
            {
                hex = "0" + hexname;
            }
            else
            {
                hex = "";
            }
            SAvgArb = ImageProperty.HexToInt(hex);
        }

        public override string ToString()
        {
            return W + "x" + H;
        }
    }
}