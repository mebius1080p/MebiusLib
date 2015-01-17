using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace MebiusLib
{
    public static class MebiusUtil
    {
        private static readonly Regex reZen = new Regex(@"[！＃＄％＆（）＊＋，－．／０-９：；＜＝＞？＠Ａ-Ｚ［］＾＿｀ａ-ｚ｛｜｝]", RegexOptions.Compiled);
        /// <summary>
        /// 全角文字を半角にして返すメソッド
        /// </summary>
        /// <param name="aStr">変換する文字列</param>
        /// <returns>全角文字が半角に変換された文字列</returns>
        public static string WideToNarrow(string aStr)
        {
            string ret = MebiusUtil.reZen.Replace(aStr, m =>
            {
                return ((char)((int)(m.Value.ToCharArray())[0] - 65248)).ToString();
            });//”と’と～はなし
            return ret.Replace("　", " ");//全角スペース=>半角スペース
        }
        /// <summary>
        /// オブジェクトの参照ではない完全コピーメソッド
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="aTarget">任意のオブジェクト</param>
        /// <returns>受け取ったものと同じ型のオブジェクト</returns>
        public static T DeepCopy<T>(T aTarget)
        {
            T ret;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, aTarget);
                ms.Position = 0;
                ret = (T)bf.Deserialize(ms);
            }
            return ret;
        }
        /// <summary>
        /// 長い文字列を指定した長さで分割して配列に納め、返すメソッド
        /// </summary>
        /// <param name="aStr">長さが 0 より大きい文字列</param>
        /// <param name="aSplitLength">分割する数。0 より大きい</param>
        /// <returns>分割して納められた文字列の配列</returns>
        public static string[] SplitStr(string aStr, int aSplitLength)
        {
            if (aStr.Length == 0)
            {
                throw new Exception("文字列の長さが 0 です");
            }
            if (aSplitLength <= 0)
            {
                throw new Exception("分割する数は 0 より大きくしてください。");
            }
            int x = aStr.Length / aSplitLength;
            int y = aStr.Length % aSplitLength;
            string[] ret;
            if (x > 0)
            {
                ret = new string[(y != 0) ? x + 1 : x];//あまりが 0 で無ければ一個余分に用意する
                int j = 0;//文字位置
                for (int i = 0; i < x; i++)
                {
                    ret[i] = aStr.Substring(j, aSplitLength);
                    j += aSplitLength;//文字位置を進める
                }
                if (y != 0)
                {//しっぽを追加
                    ret[x] = aStr.Substring(j, y);
                }
            }
            else
            {//aSplitLength 未満の文字列の時
                ret = new string[1];
                ret[0] = aStr;
            }
            return ret;
        }
    }
}
