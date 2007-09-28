using System;

namespace AnjLab.FX.System
{
    public delegate TProduct PrototypeFactoryMethod<TProduct>()
            where TProduct : ICloneable;
}
