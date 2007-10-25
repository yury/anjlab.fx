using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using AnjLab.FX.System;

namespace AnjLab.FX.StreamMapping
{
    public class BinaryMapper<TResult> 
        where TResult:class, new()
    {
        private static IMapper<TResult> _mapper = null;

        public TResult Map(byte[] data) 
        {
            Guard.ArgumentNotNull("data", data);
            Guard.ArgumentGreaterThenZero("data.Length", data.Length);

            if (_mapper == null)
                _mapper = GenerateMapper();
            return _mapper.Map(data);
        }

        private IMapper<TResult> GenerateMapper()
        {
            IMapInfo info = GetFormat(typeof(TResult));
            return (IMapper<TResult>)AssemblyBuilder.BuildMapper(info);
        }

        private IMapInfo GetFormat(Type type)
        {
            string formatName = FindFormatName(type.Assembly, type.Name);
            Guard.NotNullNorEmpty(formatName, "Binary format not found for type {0}", type.FullName);

            Stream stream = type.Assembly.GetManifestResourceStream(formatName);
            IMapInfo info = XamlReader.Load(stream) as IMapInfo;
            Guard.NotNull(info, "Error in binary format file {0}", formatName);

            info.MapedType = type;
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