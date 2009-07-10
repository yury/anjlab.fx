using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnjLab.FX.Collections;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Collections
{
    [TestFixture]
    public class PriorityQueueTests
    {
        [Test]
        public void TestCount()
        {
            PriorityQueue<int, string> q = new PriorityQueue<int, string>();
            q.Enqueue(0, "a");
            q.Enqueue(-1, "b");
            q.Enqueue(1, "c");

            Assert.AreEqual(3, q.Count);
        }

        [Test]
        public void TestOrder()
        {
            PriorityQueue<int, string> q = new PriorityQueue<int, string>();
            q.Enqueue(0, "a");
            q.Enqueue(-1, "b");
            q.Enqueue(1, "c");

            Assert.AreEqual("b", q.Dequeue());
            Assert.AreEqual("a", q.Dequeue());
            Assert.AreEqual("c", q.Dequeue());

            q.Enqueue(0, "a");
            q.Enqueue(0, "b");
            q.Enqueue(0, "c");

            q.Enqueue(1, "c");
            q.Enqueue(1, "d");
            q.Enqueue(1, "e");

            Assert.AreEqual("a", q.Dequeue());
            Assert.AreEqual("b", q.Dequeue());
            Assert.AreEqual("c", q.Dequeue());

            Assert.AreEqual("c", q.Dequeue());
            Assert.AreEqual("d", q.Dequeue());
            Assert.AreEqual("e", q.Dequeue());
        }

        [Test]
        public void TestOrderWithRandom()
        {
            PriorityQueue<int, int> q = new PriorityQueue<int, int>();
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int v = rnd.Next(-1000, 1000);
                q.Enqueue(v, v);
            }
            Assert.AreEqual(1000, q.Count);
            int prevValue = int.MinValue;
            foreach (int value in q)
            {
                Assert.IsTrue(prevValue <= value);
            }
        }

        [Test]
        public void TestPriorityDequeue()
        {
            PriorityQueue<int, int> q = new PriorityQueue<int, int>();
            q.Enqueue(1, 1);
            q.Enqueue(1, 2);
            q.Enqueue(1, 3);
            q.Enqueue(1, 4);

            q.Enqueue(10, 11);
            q.Enqueue(10, 12);
            q.Enqueue(10, 13);
            q.Enqueue(10, 14);

            Assert.AreEqual(8, q.Count);

            Assert.AreEqual(4, q.GetPriorityCount(1));
            Assert.AreEqual(1, q.Dequeue(1));
            Assert.AreEqual(2, q.Dequeue(1));
            Assert.AreEqual(3, q.Dequeue(1));
            Assert.AreEqual(4, q.Dequeue(1));
            Assert.AreEqual(0, q.GetPriorityCount(1));

            Assert.AreEqual(4, q.Count);

            Assert.AreEqual(4, q.GetPriorityCount(10));
            Assert.AreEqual(11, q.Dequeue(10));
            Assert.AreEqual(12, q.Dequeue(10));
            Assert.AreEqual(13, q.Dequeue(10));
            Assert.AreEqual(14, q.Dequeue(10));
            Assert.AreEqual(0, q.GetPriorityCount(10));
        }

        [Test]
        public void TestPeek()
        {
            PriorityQueue<int, int> q = new PriorityQueue<int, int>();
            q.Enqueue(2, 2);
            q.Enqueue(1, 1);
            Assert.AreEqual(1, q.Peek());
            Assert.AreEqual(2, q.Count);
        }
    }
}
