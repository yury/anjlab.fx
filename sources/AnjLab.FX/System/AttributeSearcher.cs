using System;
using System.Collections.Generic;
using System.Reflection;

namespace AnjLab.FX.System
{
    public class AttributeSearcher
    {
        const BindingFlags AllMembers =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

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
            return Attribute.GetCustomAttribute(minfo, typeof(TAttribute)) as TAttribute;
        }
    }
}
