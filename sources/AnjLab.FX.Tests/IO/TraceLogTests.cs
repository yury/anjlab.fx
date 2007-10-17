using AnjLab.FX.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    [TestFixture]
    public class TraceLogTests
    {
        [Test]
        public void Test()
        {
            TraceLog log = new TraceLog();
            log.Info("test info {0}", 1);
            log.Warning("test warning {0}", 2);
            log.Error("test error {0}", 3);
            log.Fatal("test fatal {0}", 4);
        }
    }
}
