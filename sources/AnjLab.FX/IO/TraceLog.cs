using System;
using System.Diagnostics;

namespace AnjLab.FX.IO
{
    public class TraceLog: ILog
    {
        public void Info(string message, params object[] args)
        {
            Trace.TraceInformation(message, args);
            InvokeLogMessage(LogLevel.INFO, string.Format(message, args));
        }

        public void Debug(string message, params object[] args)
        {
            Info(message, args);
            InvokeLogMessage(LogLevel.DEBUG, string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            Trace.TraceWarning(message, args);
            InvokeLogMessage(LogLevel.WARNING, string.Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            Trace.TraceError(message, args);
            InvokeLogMessage(LogLevel.ERROR, string.Format(message, args));
        }

        public void Fatal(string message, params object[] args)
        {
            Trace.TraceError(message, args);
            InvokeLogMessage(LogLevel.FATAL, string.Format(message, args));
        }


        public void Info(string message)
        {
            Trace.TraceInformation(message);
            InvokeLogMessage(LogLevel.INFO, message);
        }

        public void Warning(string message)
        {
            Trace.TraceWarning(message);
            InvokeLogMessage(LogLevel.WARNING, message);
        }

        public void Error(string message)
        {
            Trace.TraceError(message);
            InvokeLogMessage(LogLevel.ERROR, message);
        }

        public void Fatal(string message)
        {
            Trace.TraceError(message);
            InvokeLogMessage(LogLevel.FATAL, message);
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

        public event EventHandler<Sys.EventArgs<string, string>> LogMessage;

        private void InvokeLogMessage(string logLevel, string logMessage)
        {
            if (LogMessage != null)
            {
                LogMessage(this, Sys.EventArg.New(logLevel, logMessage));
            }
        }

    }
}
