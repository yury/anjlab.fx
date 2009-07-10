using System;
using System.IO.Ports;
using AnjLab.FX.Gsm;
using NUnit.Framework;

namespace AnjLab.FX.Tests.Gsm
{
    [TestFixture]
    public class ModemTests
    {
        readonly SerialPort port = new SerialPort("COM7"); 

        [Test, Ignore]
        public void TestATCommand()
        {
            GenericModem genericModem = new GenericModem(port);
            genericModem.Connect();
            ATResponse response = genericModem.ATCommand(new ATRequest("at+cgmi\r"), TimeSpan.FromSeconds(5));
            Assert.AreEqual(ATComandResult.Ok, response.Result);
            Console.WriteLine(response.Response);

            response = genericModem.ATCommand(new ATRequest("at+unknown\r"), TimeSpan.FromSeconds(5));
            Assert.AreEqual(ATComandResult.Error, response.Result);

            genericModem.Disconnect();
        }

        [Test, Ignore]
        public void TestSms()
        {
            GenericModem genericModem = new GenericModem(port);
            genericModem.Connect();

            ATResponse response = genericModem.SendSms(new Sms("+79206239079", "Hello Yury!"), TimeSpan.FromSeconds(5));
            Assert.AreEqual(ATComandResult.Ok, response.Result);
            Console.WriteLine(response.Response);

            genericModem.Disconnect();
        }
    }
}
