#if NET_3_5
using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using AnjLab.FX.StreamMapping.CodeGeneration;
using AnjLab.FX.Sys;

namespace AnjLab.FX.StreamMapping
{
    public class BinaryStreamMapper<TResult> : AbstractBinaryStreamMapper, IBinaryMapper<TResult>
        where TResult:class, new()
    {
        private static readonly IBinaryMapper<TResult> binaryMapper = GenerateMapper();

        public TResult Map(byte[] data) 
        {
            Guard.ArgumentNotNull("data", data);
            Guard.ArgumentGreaterThenZero("data.Length", data.Length);

            return (binaryMapper.Clone() as IBinaryMapper<TResult>).Map(data);
        }

        public override object MapObject(byte[] data)
        {
            return Map(data);
        }

        private static IBinaryMapper<TResult> GenerateMapper()
        {
            MapInfo info = GetMapInfo(typeof(TResult));
            return StreamMapperBuilder.BuildBinaryMapper<TResult>(info);
        }

        private static MapInfo GetMapInfo(Type type)
        {
            string formatName = FindMapInfo(type.Assembly, type.Name);
            Guard.NotNullNorEmpty(formatName, "Binary format not found for type {0}", type.FullName);

            Stream stream = type.Assembly.GetManifestResourceStream(formatName);
            MapInfo info = XamlReader.Load(stream) as MapInfo;
            Guard.NotNull(info, "Error in binary format file {0}", formatName);
            return info;
        }

        private static string FindMapInfo(Assembly assembly, string typeName)
        {
            typeName = typeName + ".map";
            foreach (string res in assembly.GetManifestResourceNames())
                if (res.EndsWith(typeName))
                    return res;
            return null;
        }

        public object Clone()
        {
            return new BinaryStreamMapper<TResult>();
        }
    }
}

#endif