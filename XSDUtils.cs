using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace MebiusLib
{
    /// <summary>
    /// xml ファイルを、xsd 文字列でチェックしつつ XmlDocument を返すユーティリティークラス
    /// </summary>
    class XSDUtils
    {
        /// <summary>
        /// xml ファイルパスと xsd 文字列を引数に、XmlDocument を返すメソッド
        /// </summary>
        /// <param name="aXmlFile">読み込む xml ファイルのパス</param>
        /// <param name="aXSDString">xml ファイルのスキーマとなる xsd ファイルの文字列</param>
        /// <returns>xml ファイルから構築した XmlDocument</returns>
        public static XmlDocument checkAndLoad(string aXmlFile, string aXSDString)
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
        /// <summary>
        /// xml ファイルパスと xsd 文字列を引数に、XmlDocument を返すメソッド オーバーロード
        /// </summary>
        /// <param name="aXmlFile">読み込む xml ファイルのパス</param>
        /// <param name="aXSDString">xml ファイルのスキーマとなる xsd ファイルの文字列</param>
        /// <returns>xml ファイルから構築した XmlDocument</returns>
        public static XmlDocument checkAndLoad(FileInfo aXmlFile, string aXSDString)
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
        /// <summary>
        /// スキーマチェックでエラーが出たときに実行される関数 エラーだったら例外を出す
        /// </summary>
        /// <param name="aSender">イベントを起こした元のオブジェクト</param>
        /// <param name="e">バリデーションイベントの引数</param>
        private static void xsdErrorHandler(object aSender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
