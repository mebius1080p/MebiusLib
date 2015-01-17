using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace MebiusLib
{
    class XSDUtils
    {
        public static XmlDocument CheckAndLoad(string aXmlFile, string aXSDString)
        {
            XmlSchema xs;
            XmlDocument xml = new XmlDocument();
            xml.Load(aXmlFile);
            using (StringReader sr = new StringReader(aXSDString))
            {//xml schema 自体のエラーもチェック
                xs = XmlSchema.Read(XmlReader.Create(sr), XSDUtils.xsdErrorHandler);
            }
            xml.Schemas.Add(xs);
            xml.Load(aXmlFile);
            xml.Validate(XSDUtils.xsdErrorHandler);
            return xml;
        }
        public static XmlDocument CheckAndLoad(FileInfo aXmlFile, string aXSDString)
        {//オーバーロード
            XmlSchema xs;
            XmlDocument xml = new XmlDocument();
            xml.Load(aXmlFile.FullName);
            using (StringReader sr = new StringReader(aXSDString))
            {//xml schema 自体のエラーもチェック
                xs = XmlSchema.Read(XmlReader.Create(sr), XSDUtils.xsdErrorHandler);
            }
            xml.Schemas.Add(xs);
            xml.Load(aXmlFile.FullName);
            xml.Validate(XSDUtils.xsdErrorHandler);
            return xml;
        }
        private static void xsdErrorHandler(object aSender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
