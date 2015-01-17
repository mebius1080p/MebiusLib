using System;
//using プロジェクトの名前空間を追加

namespace MebiusLib
{
    static class LogSender
    {
        public static event Action<LogEventArgs> LogEvent;
        public static void SendLog(string aLog)
        {
            App.Current.Dispatcher.Invoke(LogSender.LogEvent, new LogEventArgs(aLog));
        }
    }
}
