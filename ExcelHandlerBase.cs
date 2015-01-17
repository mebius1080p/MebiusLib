using System;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace MebiusLib
{//add reference [COM:Microsoft Excel * Object Library, COM:Microsoft Office * Object Library]
    abstract class ExcelHandlerBase : IDisposable
    {
        private bool isDisposed;
        private Excel.Application ex;
        private Excel.Workbooks wBooks;
        private Excel.Workbook wBook;
        private Excel.Sheets sheets;
        protected Excel.Worksheet wSheet;//継承クラスでいじる
        public ExcelHandlerBase(FileInfo aFile, string aSheetName)
        {
            this.isDisposed = false;
            if (!File.Exists(aFile.FullName))
            {
                throw new Exception(aFile + " が存在しません。");
            }
            this.ex = new Excel.Application();
            this.wBooks = this.ex.Workbooks;
            this.wBook = this.wBooks.Open(aFile.FullName);//ファイルがない場合エクセルは自動終了する
            this.sheets = this.wBook.Worksheets;
            foreach (Excel.Worksheet s in this.sheets)//シート数 0 のエクセルファイルは存在しないっぽい
            {//シート探索
                if (s.Name == aSheetName)
                {
                    this.wSheet = s;
                    break;
                }
            }
            if (this.wSheet == null)//シートがない場合
            {
                this.disposeAllMember();
                throw new ArgumentException("ExcelWrapper:Invalid Sheet Name");
            }
        }
        ~ExcelHandlerBase()
        {//デストラクタの場合は false?
            this.Dispose(false);
        }
        public void Dispose()
        {//using を使ったときなど
            this.Dispose(true);
            //デストラクタを呼ばないようにする
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool aIsDisposing)
        {//継承されていても破棄するため
            if (!this.isDisposed)
            {//破棄されていないとき
                if (aIsDisposing)
                {//using を使ったときなど
                    this.disposeAllMember();
                    this.isDisposed = true;
                }
            }
        }
        protected void checkDisposed()
        {
            if (this.isDisposed)
            {
                throw new Exception("オブジェクトは破棄されています。");
            }
        }
        private void disposeAllMember()
        {
            if (this.wSheet != null)
            {
                Marshal.FinalReleaseComObject(this.wSheet);
                this.wSheet = null;
            }
            if (this.sheets != null)
            {
                Marshal.FinalReleaseComObject(this.sheets);
                this.sheets = null;
            }
            if (this.wBook != null)
            {
                this.wBook.Close();
                Marshal.FinalReleaseComObject(this.wBook);
                this.wBook = null;
            }
            if (this.wBooks != null)
            {
                this.wBooks.Close();
                Marshal.FinalReleaseComObject(this.wBooks);
                this.wBooks = null;
            }
            if (this.ex != null)
            {
                this.ex.Quit();
                Marshal.FinalReleaseComObject(this.ex);
                this.ex = null;
            }
            GC.Collect();
        }
    }
}
