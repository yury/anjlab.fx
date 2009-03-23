using System;
using AnjLab.FX.Sys;

namespace AnjLab.FX.IO
{
    public interface ILog
    {
        void Info(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Debug(string message, params object[] args);

        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);
        void Debug(string message);

        bool IsInfoEnabled { get;}
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsDebugEnabled { get; }

        event EventHandler<EventArgs<string, string>> LogMessage;
    }
}
