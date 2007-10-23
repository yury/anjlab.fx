using System.Collections.Generic;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public interface IObjectFactory
    {
        object New(object[] args);
        void Init(IDictionary<string, object> initData);
    }
}
