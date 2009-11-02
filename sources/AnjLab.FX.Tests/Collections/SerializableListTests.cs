using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using AnjLab.FX.Collections;
using NUnit.Framework;

#if NET_3_5
namespace AnjLab.FX.Tests.Collections
{
    [TestFixture]
    public class SerializableListTests
    {
        [Test]
        public void TestSerializationDeserialization()
        {
            SerializableList<ITest> list = new SerializableList<ITest>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializableList<ITest>));
            StringBuilder xml = new StringBuilder();

            list.Add(new Test1("str1", 4));
            list.Add(new Test1("str2", -1));
            list.Add(new Test1("str3", 0));
            xmlSerializer.Serialize(new StringWriter(xml), list);

            Console.Write(xml);

            SerializableList<ITest> list2 = (SerializableList<ITest>)xmlSerializer.Deserialize(new StringReader(xml.ToString()));
            
            for(int i = 0; i < list.Count; i++)
                Assert.AreEqual(list[i], list2[i]);
        }

    }

    public interface ITest
    {
        string Str { get; }
        int Int { get; }
    }

    [Serializable]
    public class Test1 : ITest, IEquatable<Test1>
    {
        private string _str = "str";
        private int _int = 0;

        public Test1()
        {
        }

        public Test1(string str, int @int)
        {
            _str = str;
            _int = @int;
        }

        [XmlAttribute]
        public string Str
        {
            get { return _str; }
            set { _str = value;}
        }

        [XmlAttribute]
        public int Int
        {
            get { return _int; }
            set { _int = value;}
        }


        public bool Equals(Test1 test1)
        {
            if (test1 == null) return false;
            return Equals(_str, test1._str) && _int == test1._int;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Test1);
        }

        public override int GetHashCode()
        {
            return (_str != null ? _str.GetHashCode() : 0) + 29*_int;
        }
    }
}
#endif