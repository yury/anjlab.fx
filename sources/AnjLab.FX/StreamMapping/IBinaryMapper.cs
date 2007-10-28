namespace AnjLab.FX.StreamMapping
{
    /// <summary>
    /// Implementations are dynamically generated at runtime by AssebmlyBuilder
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IBinaryMapper<TResult>
    {
        TResult Map(byte[] data);
    }
}
