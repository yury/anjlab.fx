using System;
using AnjLab.FX.IO;
using System.IO;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Log4Net
{
    public class Log4NetLog : ILog
    {
        private readonly log4net.ILog _log;
        private const string RelativePathToConfig = @"Properties\log4net.config";

        public static void LoadLog4NetConfig()
        {
            LoadLog4NetConfig(RelativePathToConfig);
        }

        public static void LoadLog4NetConfig(string relativePathToConfig)
        {
            string pathToConfig = Path.Combine(Path.GetDirectoryName(typeof(Log4NetLog).Assembly.Location), relativePathToConfig);
            if (!File.Exists(pathToConfig))
            {

                pathToConfig = Path.Combine(Environment.CurrentDirectory, relativePathToConfig);
            }

            if (!File.Exists(pathToConfig))
            {
                pathToConfig = Path.Combine(Path.GetDirectoryName(new Uri(typeof(Log4NetLog).Assembly.CodeBase).AbsolutePath), relativePathToConfig);
            }

            var fi = new FileInfo(pathToConfig);
            if (fi.Exists)
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            } 
            else
            {
                log4net.Config.BasicConfigurator.Configure();
            }
        }

        public Log4NetLog(string name)
        {
            _log = log4net.LogManager.GetLogger(name);
        }

        public void Info(string message, params object[] args)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat(message, args);
        }

        public void Debug(string message, params object[] args)
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

        public event EventHandler<EventArgs<string, string>> LogMessage;
    }
}
