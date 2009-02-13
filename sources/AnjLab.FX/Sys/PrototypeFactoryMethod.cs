using System;

namespace AnjLab.FX.Sys
{
    public delegate TProduct PrototypeFactoryMethod<TProduct>()
            where TProduct : ICloneable;
}
