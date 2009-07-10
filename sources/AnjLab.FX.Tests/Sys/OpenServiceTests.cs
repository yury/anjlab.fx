using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class OpenServiceTests
    {
        [Test]
        public void TestGetInfoForNonRegisteredService()
        {
            OpenService os = new OpenService();
            os.ServiceName = "doesn't exists";
            OpenService.ServiceInfo info = os.GetInfo();
            Assert.IsNotNull(info);
            Assert.IsFalse(info.Installed);
            Assert.AreEqual(0, (int)info.WinState);
            Assert.AreEqual(os.ServiceName, info.Name);
            Assert.IsNull(info.DisplayName);
        }

        [Test]
        public void TestGetInfoForStandartService()
        {
            OpenService os = new OpenService();
            os.ServiceName = "Workstation";
            OpenService.ServiceInfo info = os.GetInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Installed);
            Assert.AreEqual("Workstation", info.Name);
            Assert.AreEqual(ServiceControllerStatus.Running, info.WinState);
            Assert.IsNotNull(info.DisplayName);
            Console.WriteLine(info.DisplayName);
        }
    }
}
