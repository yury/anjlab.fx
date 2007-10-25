using System;

namespace AnjLab.FX.StreamMapping
{
    public interface IMapper<TResult>
    {
        TResult Map(byte[] data);
    }
}
