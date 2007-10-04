using System;
using System.Collections.Generic;
using System.Reflection;
using AnjLab.FX.Properties;

namespace AnjLab.FX.System
{
    class TypeReflector
    {
        private Dictionary<Type, object> _wellKnownTypes = new Dictionary<Type, object>();

        public TypeReflector()
        {
            Type[] types = new Type[]
                {
                    typeof(string),
                    typeof(int),
                    typeof(float),
                    typeof(double),
                    typeof(short),
                    typeof(bool),
                    typeof(Int64),
                    typeof(Nullable<int>),
                    typeof(Nullable<float>),
                    typeof(Nullable<double>),
                    typeof(Nullable<short>),
                    typeof(Nullable<bool>),
                    typeof(Nullable<Int64>)
                };
            foreach (Type type in types)
            {
                _wellKnownTypes.Add(type, null);
            }
        }

        public List<Type> WellKnownTypes
        {
            get
            {
                return new List<Type>(_wellKnownTypes.Keys);
            }
        }

        public List<Type> GetAllTypes(Type type)
        {
            Dictionary<Type, object> types = new Dictionary<Type, object>();
            FillMembersType(type, types);
            return new List<Type>(types.Keys);
        }

        private void FillMembersType(Type type, Dictionary<Type, object> types)
        {
            if (types.ContainsKey(type)
                || _wellKnownTypes.ContainsKey(type))
                return;

            types.Add(type, null);

            if (type.IsEnum)
                return;

            if (type.IsArray)
            {
                FillMembersType(type.GetElementType(), types);
                return;
            }

            BindingFlags bf = BindingFlags.Default
                              | BindingFlags.Instance
                              | BindingFlags.Public
                              | BindingFlags.FlattenHierarchy;
            //| BindingFlags.NonPublic;

            FillMembersType(type.GetFields(bf), types);
            FillMembersType(type.GetProperties(bf), types);
        }

        private void FillMembersType(IEnumerable<FieldInfo> members, Dictionary<Type, object> types)
        {
            foreach (FieldInfo info in members)
            {
                if (types.ContainsKey(info.FieldType)
                 || _wellKnownTypes.ContainsKey(info.FieldType))
                    continue;

                types.Add(info.FieldType, null);
                FillMembersType(info.FieldType, types);
            }
        }

        private void FillMembersType(IEnumerable<PropertyInfo> members, Dictionary<Type, object> types)
        {
            foreach (PropertyInfo info in members)
            {
                if (types.ContainsKey(info.PropertyType)
                 || _wellKnownTypes.ContainsKey(info.PropertyType))
                    continue;

                types.Add(info.PropertyType, null);
                FillMembersType(info.PropertyType, types);
            }
        }

        internal static bool TypeIsCollection(Type type, out Type elementType)
        {
            elementType = null;
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition().GetInterface("ICollection`1") != null
                 || type.GetGenericTypeDefinition().FullName.Equals("System.Collections.Generic.ICollection`1"))
                {
                    elementType = type.GetGenericArguments()[0];
                    return true;
                }
            }
            return false;
        }

        internal static bool TypeIsDictionary(Type type, out Type keyType, out Type valueType)
        {
            keyType = null;
            valueType = null;
            if (type.IsGenericType)
                if (type.GetGenericTypeDefinition().GetInterface("IDictionary`2") != null
                 || type.GetGenericTypeDefinition().FullName.Equals("System.Collections.Generic.IDictionary`2"))
                {
                    Type[] genericArgs = type.GetGenericArguments();
                    keyType = genericArgs[0];
                    valueType = genericArgs[1];
                    return true;
                }

            return false;
        }

        public static Type GetMemberType(Type type, string memberName)
        {
            if (string.IsNullOrEmpty(memberName) || memberName == "this")
                return type;

            string[] members = memberName.Split('.');
            foreach (string member in members)
            {
                FieldInfo fi = type.GetField(member);
                if (fi != null)
                    type = fi.FieldType;
                else
                {
                    PropertyInfo pi = type.GetProperty(member);
                    if (pi != null)
                        type = pi.PropertyType;
                    else
                        throw new ArgumentException(string.Format(Resources.CantFindMember_Name, memberName));
                }
            }

            return type;
        }


    }
}
