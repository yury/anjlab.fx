namespace AnjLab.FX.IO
{
    public interface ILog
    {
        void Info(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Debug(string message, params object[] args);

        bool IsInfoEnabled { get;}
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsDebugEnabled { get; }
    }
}
