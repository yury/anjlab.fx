using System;
using System.Collections.Generic;
using System.Linq;
using AnjLab.FX.System;
using NUnit.Framework;

namespace AnjLab.FX.Tests.System
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
    }
}
