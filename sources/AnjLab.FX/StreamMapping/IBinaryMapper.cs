using System;

namespace AnjLab.FX.StreamMapping
{
    /// <summary>
    /// Implementations are dynamically generated at runtime by AssebmlyBuilder
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IBinaryMapper<TResult> : ICloneable
    {
        TResult Map(byte[] data);
    }
}
