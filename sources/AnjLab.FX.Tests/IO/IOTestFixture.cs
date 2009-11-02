using System.IO;
using NUnit.Framework;

namespace AnjLab.FX.Tests.IO
{
    public class IOTestFixture : AssertionHelper
    {
        protected static void ExpectPosition(int expectedPos, MemoryStream stream)
        {
            Assert.AreEqual(expectedPos, stream.Position);
        }
    }
}
