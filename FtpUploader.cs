using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MebiusLib
{
    class FtpUploader
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool SslMode { get; set; }
        public FtpUploader()
        {
            this.UserName = "";
            this.PassWord = "";
            this.SslMode = true;//デフォルト
        }
        public FtpUploader(string aSearchStr)
        {//オーバーロード：FileZilla から credential を読む場合
            if (aSearchStr == "")
            {
                throw new Exception("第一引数が空文字です。");
            }
            FileZilla fz = new FileZilla(aSearchStr);
            this.UserName = fz.UserName;
            this.PassWord = fz.PassWord;
            this.SslMode = true;//デフォルト
        }
        private FtpWebRequest genFtpReq(Uri aUri)
        {
            if (this.UserName == "" || this.PassWord == "")
            {
                throw new Exception("ユーザー名またはパスワードが空文字です。");
            }
            //FtpWebRequest のセットアップ
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(aUri);
            req.Credentials = new NetworkCredential(this.UserName, this.PassWord);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.KeepAlive = false;//単発アップロード
            req.UseBinary = false;//テキストです
            req.UsePassive = true;//ルーター越し
            req.EnableSsl = this.SslMode;
            return req;
        }
        public string uploadUTF8(FileInfo aUpFile, Uri aTarget)
        {
            string ret = "";
            FtpWebRequest req = this.genFtpReq(new Uri(aTarget, aUpFile.Name));

            using (StreamReader sourceStream = new StreamReader(aUpFile.FullName))
            {//ファイルの内容読み取り
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                req.ContentLength = fileContents.Length;
                using (Stream requestStream = req.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
            }
            //レスポンスコード受け取り
            using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
            {
                ret = response.StatusDescription;
            }
            return ret;
        }
        public async Task<string> uploadUTF8Async(FileInfo aUpFile, Uri aTarget)
        {
            string ret = "";
            FtpWebRequest req = this.genFtpReq(new Uri(aTarget, aUpFile.Name));

            using (StreamReader sourceStream = new StreamReader(aUpFile.FullName))
            {//ファイルの内容読み取り
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                req.ContentLength = fileContents.Length;
                using (Stream requestStream = await req.GetRequestStreamAsync())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
            }
            //レスポンスコード受け取り
            using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
            {
                ret = response.StatusDescription;
            }
            return ret;
        }
        private class FileZilla
        {
            public string UserName { get; private set; }
            public string PassWord { get; private set; }
            private static string FZ_XSD = @"<?xml version='1.0' encoding='UTF-8'?><xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'><xsd:element name='FileZilla3'><xsd:complexType><xsd:sequence><xsd:element ref='Servers'/></xsd:sequence></xsd:complexType></xsd:element><xsd:element name='Servers'><xsd:complexType><xsd:sequence><xsd:element ref='Server' minOccurs='1' maxOccurs='unbounded'/></xsd:sequence></xsd:complexType></xsd:element><xsd:element name='Server'><xsd:complexType mixed='true'><xsd:sequence><xsd:element name='Host' type='xsd:string'/><xsd:element name='Port' type='xsd:string'/><xsd:element name='Protocol' type='xsd:string'/><xsd:element name='Type' type='xsd:string'/><xsd:element name='User' type='xsd:string'/><xsd:element name='Pass' type='xsd:string'/><xsd:element name='Logontype' type='xsd:string'/><xsd:element name='TimezoneOffset' type='xsd:string'/><xsd:element name='PasvMode' type='xsd:string'/><xsd:element name='MaximumMultipleConnections' type='xsd:string'/><xsd:element name='EncodingType' type='xsd:string'/><xsd:element name='BypassProxy' type='xsd:string'/><xsd:element name='Name' type='xsd:string'/><xsd:element name='Comments' type='xsd:string'/><xsd:element name='LocalDir' type='xsd:string'/><xsd:element name='RemoteDir' type='xsd:string'/><xsd:element name='SyncBrowsing' type='xsd:string'/></xsd:sequence></xsd:complexType></xsd:element></xsd:schema>";
            public FileZilla(string aSearchStr)
            {
                this.UserName = "";
                this.PassWord = "";
                string fzFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FileZilla\\sitemanager.xml");
                this.getInfo(fzFilePath, aSearchStr);
            }
            private void getInfo(string aFile, string aStr)
            {
                XmlDocument xml = XSDUtils.checkAndLoad(aFile, FileZilla.FZ_XSD);
                XmlNodeList server = xml.GetElementsByTagName("Server");
                foreach (XmlNode s in server)
                {
                    if (s.SelectSingleNode("Host").FirstChild.Value.Contains(aStr) == true)
                    {
                        this.UserName = s.SelectSingleNode("User").FirstChild.Value;
                        this.PassWord = s.SelectSingleNode("Pass").FirstChild.Value;
                        Encoding enc = Encoding.GetEncoding("UTF-8");
                        this.PassWord = enc.GetString(Convert.FromBase64String(this.PassWord));
                        break;
                    }
                }
                if (this.UserName == "" || this.PassWord == "")
                {
                    throw new Exception("ユーザー名またはパスワードが空文字です。");
                }
            }
        }
    }
}
