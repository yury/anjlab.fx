using System;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Web
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TemplateElementAttribute : MemberAttribute
    {
        private readonly string _id = "";
        bool _bindOnLoad = false;
        bool _canBeNull = false;

        public TemplateElementAttribute()
        {
        }

        public TemplateElementAttribute(string id)
        {
            _id = id;
        }

        public string ID
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                    return Member.Name;
                return _id;
            }
        }

        public bool BindOnLoad
        {
            get { return _bindOnLoad; }
            set { _bindOnLoad = value; }
        }

        public bool CanBeNull
        {
            get { return _canBeNull; }
            set { _canBeNull = value; }
        }
    }
}
