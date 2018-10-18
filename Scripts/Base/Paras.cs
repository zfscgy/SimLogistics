using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimLogistics.Paras
{
    using Base;
    public static class TransportationParas
    {
        public static Dictionary<TransportationType, float> TransportationSpeed = new Dictionary<TransportationType, float>
        {
            { TransportationType.helicopter, 100 },
            { TransportationType.truck, 15 },
            { TransportationType.pickup, 20 },
        };
        public static Dictionary<TransportationType, float> TransportationCapacity = new Dictionary<TransportationType, float>
        {
            { TransportationType.helicopter, 1000 },
            { TransportationType.truck, 10000 },
            { TransportationType.pickup, 1000 },
        };
    }
}
