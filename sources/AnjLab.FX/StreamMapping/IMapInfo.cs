using System;
using System.Collections.Generic;

namespace AnjLab.FX.StreamMapping
{
    public interface IMapInfo
    {
        Type MapedType { get; set; }

        List<IMapInfoElement> Elements { get; }
    }
}