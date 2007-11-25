using AnjLab.FX.System;
using NUnit.Framework;

namespace AnjLab.FX.Tests.System
{
    [TestFixture]
    public class CommandArgsProcessorTests
    {
        [Test]
        public void TestMappings()
        {
            string[] args = new string[] {"/oneKey", "/secondKey"};

            bool oneKeyExecuted = false;

            CommandArgsProcessor proc = new CommandArgsProcessor(args);
            proc.MapKey("onekey", Command.FromAction(delegate { oneKeyExecuted = true; }));
            proc.MapKey("secondkey", Command.FromAction(delegate { Assert.Fail("Should only execute first key");}));

            proc.Run();

            Assert.IsTrue(oneKeyExecuted);

            bool defaultExecuted = false;
            proc = new CommandArgsProcessor(new string[0]);
            proc.MapNoKey(delegate
                                {
                                    defaultExecuted = true;
                                });
            proc.MapKey("someKey", delegate { Assert.Fail("should run only defualt.");});
            proc.MapKey("secondKey", delegate { Assert.Fail("should run only defualt."); });

            proc.Run();

            Assert.IsTrue(defaultExecuted);
        }
    }
}
