namespace AnjLab.FX.System
{
    public delegate TProduct FactoryMethod<TProduct>();
    public delegate TProduct FactoryMethod<TProduct, TSource>(TSource src);
}
