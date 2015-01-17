using System;

namespace MebiusLib
{
    class LogEventArgs : EventArgs
    {
        public string Log { get; private set; }
        public LogEventArgs(string aLog)
        {
            this.Log = aLog;
        }
    }
}
