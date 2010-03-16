using System;
using System.IO;
using NUnit.Framework;
using AnjLab.FX.MSBuild.Tasks;

namespace AnjLab.FX.Tests.MsBuild
{
    [TestFixture]
    public class ArchiveFolderTaskTests
    {
        [Test]
        public void TestFind7Zip()
        {
            Console.WriteLine(ArchiveFolder._7Zip);
            Assert.IsTrue(File.Exists(ArchiveFolder._7Zip));
        }
    }
}
