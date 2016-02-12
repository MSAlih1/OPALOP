using QPS_Web1._QPS.Type;
using System;
using System.IO;

namespace QPS_Web1._QPS
{
    public class qprPath
    {
        public qprPath(string _GetUserName)
        {
            UserName = _GetUserName;
        }

        public string ServerIP
        {
            get
            { return "api.qpiro.com"; }
        }

        public string InQueueList
        {
            get { return "InQueueList.txt"; }
        }

        public string BlackJPG
        {
            get { return "black.jpg"; }
        }

        public string UserXmlInfo
        {
            get { return "UserInfo.xml"; }
        }

        public bool EmailVerified
        {
            get
            {
                //Client cn = UserProperty.ConnectAuth();
                //UserProfile up = cn.GetUser(ClaimsPrincipal.Current.Identity.Name);
                //string isValid = up.ExtraProperties["email_verified"].ToString();
                //if (isValid == "True")
                //{
                //    return true;
                //}
                //else
                //{
                return true;
                //}
            }
        }

        public string Startup_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserFolders");

        private string _username;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Current_User
        { get { return Path.Combine(Startup_Path, UserName); } }

        public string Data_Path
        { get { return Path.Combine(Current_User, "Data"); } }

        public string Data_InstagramPhotos
        { get { return Path.Combine(Data_Path, PixFormat._94x94.ToString(), "Instagram"); } }

        public string Data_FacebookPhotos
        { get { return Path.Combine(Data_Path, PixFormat._94x94.ToString(), "Facebook"); } }

        public string PixelXmlMap_Path
        { get { return Path.Combine(Data_Path, "PixelXmlMap"); } }

        public string SavedPhotos_Path
        { get { return Path.Combine(Data_Path, "SavedPhotos"); } }

        public string ResourcePhotos_Path
        { get { return Path.Combine(Data_Path, "ResourcePhotos"); } }
    }
}