using System;
using AnjLab.FX.IO;

namespace AnjLab.FX.Tools.IO
{
    class Log4NetLog : ILog
    {
        private readonly log4net.ILog _log;

        public Log4NetLog(string name)
        {
            _log = log4net.LogManager.GetLogger(name);
        }

        public void Info(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat(message, args);
        }

        public void Debug(string message, params object [] args)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            if (_log.IsWarnEnabled)
                _log.WarnFormat(message, args);
        }
            
        public void Error(string message, params object[] args)
        {
            if (_log.IsErrorEnabled)
                _log.ErrorFormat(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            if (_log.IsFatalEnabled)
                _log.FatalFormat(message, args);
        }


        public void Info(string message)
        {
            if (_log.IsInfoEnabled)
                _log.Info(message);
        }

        public void Warning(string message)
        {
            if (_log.IsWarnEnabled)
                _log.Warn(message);
        }

        public void Error(string message)
        {
           if (_log.IsErrorEnabled)
               _log.Error(message);
        }

        public void Fatal(string message)
        {
            if (_log.IsFatalEnabled)
                _log.Fatal(message);
        }

        public void Debug(string message)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message);
        }

        public bool IsInfoEnabled
        {
            get { return _log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _log.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _log.IsFatalEnabled; }
        }

        public bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }
    }
}
