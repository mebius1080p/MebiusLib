using MSHTML;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MebiusLib
{
    /// <summary>
    /// Download and parse html file on WWW with mshtml.
    /// Prerequisites : Laucn VS Dev tool command prompt and do following.
    /// cd [temp directory you want]
    /// tlbimp c:\windows\system32\mshtml.tlb
    /// After while, huge MSHTML.dll is created.
    /// Copy MSHTML.dll to your project directory. (ex where .sln file is placed
    /// If your project already has referrence of com mshtml, remove reference.
    /// Add reference of MSHTML.dll in you project directory.
    /// Make sure implant interoperability settings of MSHTML reference to false.
    /// Inherit this class and enjoy.
    /// In this method, you can use new API like querySelector but you need to cast object to some interface, IHTMLElement2 for example.
    /// It will bother you but new API is fun any way.
    /// Caution : querySelector and querySelectorAll can have old IE8 CSS expression.
    /// You can use #foobar>div:first-child but #foobar>last-child not.
    /// </summary>
    abstract class ParseHTMLBase : IDisposable
    {
        protected HTMLDocumentClass hdoc;
        private bool isDisposed;
        public ParseHTMLBase()
        {
            this.isDisposed = false;
            this.hdoc = new HTMLDocumentClass();
            IPersistStreamInit ips = (IPersistStreamInit)hdoc;
            ips.InitNew();
        }
        ~ParseHTMLBase()
        {
            this.Dispose(false);
        }
        public void Dispose()
        {
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
                throw new Exception("This object is already disposed.");
            }
        }
        private void disposeAllMember()
        {
            if (this.hdoc != null)
            {
                Marshal.FinalReleaseComObject(this.hdoc);
                this.hdoc = null;
            }
            GC.Collect();
        }
    }
    [ComImport(), Guid("0000010c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPersist
    {
        void GetClassID(Guid pClassId);
    }
    [ComImport(), Guid("7FD52380-4E07-101B-AE2D-08002B2EC713"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPersistStreamInit : IPersist
    {
        void GetClassID([In, Out] ref Guid pClassId);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig()]
        int IsDirty();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig()]
        void Load(IStream pStm); //System.Runtime.InteropServices.ComTypes.IStream
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig()]
        void Save(IStream pStm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);//System.Runtime.InteropServices.ComTypes.IStream
        void GetMaxSize([Out]long pCbSize);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig()]
        void InitNew();
    }
}
