using QPS_Method1._QPS;
using QPS_Web1._QPS;
using QPS_Web1._QPS.Class;
using QPS_Web1._QPS.Type;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace QPS_Method1
{
    /// <summary>
    /// Summary description for Processing
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class Processing : System.Web.Services.WebService
    {
        [WebMethod]
        public bool CreateDir(string AccessKey, string UserName)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                qpsSystem.CreateDir(UserName);
                return true;
            }
            return false;
        }

        [WebMethod]
        public bool CreateXml(string AccessKey, string UserName)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                qpsSystem.CreateXml(UserName);
                return true;
            }
            return false;
        }

        [WebMethod]
        public bool DownloadInstaPhotos(string AccessKey, string UserName, string[] photos)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                qpsSystem.DownloadInstaPhotos(UserName, photos);
                return true;
            }
            return false;
        }

        [WebMethod]
        public NewImagePart ImageGenerate(string AccessKey, string UserName, byte[] ImagePart, int x, int y, int width, int height, int PxFormat)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                return qpsSystem.PointGenerator(UserName, ImagePart, x, y, width, height, PxFormat);
            }
            return null;
        }

        [WebMethod]
        public bool XmlUpdate(string AccessKey, string UserName, string key, int _Value, bool ValueArtir)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                qpsSystem.XmlUpdate(UserName, key, _Value, ValueArtir);
                return true;
            }
            return false;
        }
        [WebMethod]
        public bool SelectedInstaPhotos(string AccessKey, string UserName, string ls)
        {
            if (AccessKey == qpsSystem.GetAccessCode())
            {
                qpsSystem.UpdateSelectedInstaPhotos(UserName, ls);
                return true;
            }
            return false;
        }
    }
}