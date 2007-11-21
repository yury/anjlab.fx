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
    }
}
