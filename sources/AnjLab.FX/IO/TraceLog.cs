using System;
using System.Diagnostics;

namespace AnjLab.FX.IO
{
    public class TraceLog: ILog
    {
        public void Info(string message, params object[] args)
        {
            Trace.TraceInformation(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            Info(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            Trace.TraceWarning(message, args);
        }

        public void Error(string message, params object[] args)
        {
            Trace.TraceError(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            Trace.TraceError(message, args);
        }


        public void Info(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
        }

        public void Fatal(string message)
        {
            Trace.TraceError(message);
        }

        public void Debug(string message)
        {
            Info(message);
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }
    }
}
