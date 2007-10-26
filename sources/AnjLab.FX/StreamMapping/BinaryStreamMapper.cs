using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class BinaryStreamMapper<TResult> 
        where TResult:class, new()
    {
        private static IBinaryMapper<TResult> binaryMapper = null;

        public TResult Map(byte[] data) 
        {
            Guard.ArgumentNotNull("data", data);
            Guard.ArgumentGreaterThenZero("data.Length", data.Length);

            if (binaryMapper == null)
                binaryMapper = GenerateMapper();
            return binaryMapper.Map(data);
        }

        private IBinaryMapper<TResult> GenerateMapper()
        {
            MapInfo info = GetFormat(typeof(TResult));
            return AssemblyBuilder.BuildBinaryMapper<TResult>(info);
        }

        private MapInfo GetFormat(Type type)
        {
            string formatName = FindFormatName(type.Assembly, type.Name);
            Guard.NotNullNorEmpty(formatName, "Binary format not found for type {0}", type.FullName);

            Stream stream = type.Assembly.GetManifestResourceStream(formatName);
            MapInfo info = XamlReader.Load(stream) as MapInfo;
            Guard.NotNull(info, "Error in binary format file {0}", formatName);
            return info;
        }

        private string FindFormatName(Assembly assembly, string typeName)
        {
            typeName = typeName + ".binmap";
            foreach (string res in assembly.GetManifestResourceNames())
                if (res.EndsWith(typeName))
                    return res;
            return null;
        }
    }
}