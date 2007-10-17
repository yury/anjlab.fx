namespace AnjLab.FX.IO
{
    public interface ILog
    {
        void Info(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
    }
}
