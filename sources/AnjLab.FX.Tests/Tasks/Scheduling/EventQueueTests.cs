using System;
using System.Threading;
using AnjLab.FX.Sys;
using AnjLab.FX.Tasks.Scheduling;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Tasks.Scheduling
{
    [TestFixture] 
    public class EventQueueTests
    {
        [Test]
        public void TestEvents()
        {
            int occursCount = 0;
            EventQueue q = new EventQueue();
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(1)));
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(1)));
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(2)));
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(2)));
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(1)));
            q.Register(Trigger.Once("once", DateTime.Now.AddSeconds(2)));
            q.EventOccurs += delegate(object sender, EventArgs<string> e) 
            {
                Assert.AreEqual("once", e.Item);
                occursCount++;
            };

            q.Start(true);
            Thread.Sleep(TimeSpan.FromSeconds(3));
            Assert.AreEqual(6, occursCount);
            q.Stop();
            Thread.Sleep(100);
        }
    }
}
