namespace AnjLab.FX.Sys
{
    public delegate TProduct FactoryMethod<TProduct>();
    public delegate TProduct FactoryMethod<TProduct, TSource>(TSource src);
}
