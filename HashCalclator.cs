using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MebiusLib
{
    class HashCalclator
    {
        public string Hash { get; private set; }
        public HashCalclator(string aFile)
        {
            if (aFile == string.Empty)
            {
                throw new Exception("引数が空文字です");
            }
            this.Hash = "";
            using (FileStream fs = new FileStream(aFile, FileMode.Open, FileAccess.Read))
            {
                using (SHA256Cng sha = new SHA256Cng())
                {
                    byte[] hash = sha.ComputeHash(fs);
                    StringBuilder sb = new StringBuilder(128);
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    this.Hash = sb.ToString();
                }
            }
        }
    }
}
