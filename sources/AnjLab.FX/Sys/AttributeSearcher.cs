using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AnjLab.FX.Sys
{
    public class AttributeSearcher
    {
        const BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        const BindingFlags DeclaredOnlyMembers = AllMembers | BindingFlags.DeclaredOnly;

        public static IList<TAttribute> GetMemberAttributes<TAttribute>(Type type)
         where TAttribute : MemberAttribute
        {
            return GetMemberAttributes<TAttribute>(type, false);
        }

        public static IList<TAttribute> GetMemberAttributes<TAttribute>(Type type, bool useParentMembers)
           where TAttribute : MemberAttribute
        {
            IList<TAttribute> result = new List<TAttribute>();
            foreach (MemberInfo minfo in type.GetMembers((useParentMembers) ? AllMembers : DeclaredOnlyMembers))
            {
                TAttribute attribute = GetMemberAttribute<TAttribute>(minfo);
                if (attribute != null)
                    result.Add(attribute);
            }
            return result;
        }

        public static TAttribute GetMemberAttribute<TAttribute>(MemberInfo minfo)
            where TAttribute : MemberAttribute
        {
            TAttribute attribute = GetAttribute<TAttribute>(minfo);
            if (attribute != null)
                attribute.Member = minfo;
            return attribute;
        }

        public static TAttribute GetAttribute<TAttribute>(MemberInfo minfo)
           where TAttribute : Attribute
        {
            long key = GetCacheKey(typeof(TAttribute), minfo);

            if (ContainsInCache(key))
                return GetFromCache<TAttribute>(key);

            lock (_cache)
            {
                if (ContainsInCache(key))
                    return GetFromCache<TAttribute>(key);

                TAttribute attr = Attribute.GetCustomAttribute(minfo, typeof (TAttribute)) as TAttribute;
                AddToCache(key, attr);
                return attr;
            }
        }

        private static void AddToCache<TAttribute>(long key, TAttribute atr)
             where TAttribute : Attribute
        {
            _cache.Add(key, atr);
        }

        private static TAttribute GetFromCache<TAttribute>(long key)
             where TAttribute : Attribute
        {
            return _cache[key] as TAttribute;
        }

        private static bool ContainsInCache(long key)
        {
            return _cache.ContainsKey(key);
        }

        private static long GetCacheKey(Type type, MemberInfo minfo)
        {
            long key;
            key = minfo.MetadataToken;
            key = key << 32;
            key += type.GetHashCode();
            return key;
        }

        private static Hashtable _cache = new Hashtable();
    }
}
