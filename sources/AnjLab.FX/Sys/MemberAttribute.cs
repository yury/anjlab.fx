using System;
using System.Reflection;
using AnjLab.FX.Properties;

namespace AnjLab.FX.Sys
{
    public class MemberAttribute : Attribute
    {
        private MemberInfo _member;


        public object this[object obj]
        {
            get
            {
                FieldInfo field = _member as FieldInfo;
                if (field != null)
                    return field.GetValue(obj);

                PropertyInfo property = _member as PropertyInfo;
                if (property != null)
                    return property.GetValue(obj, null);

                throw new InvalidOperationException(Resources.CanGetValueOnlyForFieldOrPropertyMember);
            }
            set
            {
                FieldInfo field = _member as FieldInfo;
                if (field != null)
                    field.SetValue(obj, value);
                else
                {
                    PropertyInfo property = _member as PropertyInfo;
                    if (property != null)
                        property.SetValue(obj, value, null);
                    else
                        throw new InvalidOperationException(Resources.CanSetValueOnlyForFieldOrPropertyMember);
                }
            }
        }

        public MemberInfo Member
        {
            get { return _member; }
            set { _member = value; }
        }
    }
}
