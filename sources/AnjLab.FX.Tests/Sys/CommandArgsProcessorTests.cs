using AnjLab.FX.Sys;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Sys
{
    [TestFixture]
    public class CommandArgsProcessorTests
    {
        [Test]
        public void TestMappings()
        {
            string[] args = new string[] { "/oneKey", "/secondKey" };

            bool oneKeyExecuted = false;

            CommandArgsProcessor proc = new CommandArgsProcessor(args);
            proc.MapKey("onekey", Command.FromAction(delegate { oneKeyExecuted = true; }));
            proc.MapKey("secondkey", Command.FromAction(delegate { Assert.Fail("Should only execute first key"); }));

            proc.Run();

            Assert.IsTrue(oneKeyExecuted);

            bool defaultExecuted = false;
            proc = new CommandArgsProcessor(new string[0]);
            proc.MapNoKey(delegate
            {
                defaultExecuted = true;
            });
            proc.MapKey("someKey", delegate { Assert.Fail("should run only defualt."); });
            proc.MapKey("secondKey", delegate { Assert.Fail("should run only defualt."); });

            proc.Run();

            Assert.IsTrue(defaultExecuted);
        }

        [Test]
        public void TestParams()
        {
            var args = new[] { "/p:username=user", "/p:password=secret" };

            var proc = new CommandArgsProcessor(args);

            Assert.That(proc.HasParam("username"));
            Assert.AreEqual("user", proc.GetParamValue("username"));

            Assert.That(proc.HasParam("password"));
            Assert.AreEqual("secret", proc.GetParamValue("password"));
        }

        [Test]
        public void TestKeyInsensitiveParamKeys()
        {
            var args = new[] { "/P:username=user" };

            var proc = new CommandArgsProcessor(args);

            Assert.That(proc.HasParam("UserName"));
            Assert.AreEqual("user", proc.GetParamValue("USERNAME"));
        }
    }
}