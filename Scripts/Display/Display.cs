using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace SimLogistics.Display
{
    using Common;
    public interface DisplayableBase
    {
        Vector3 GetPosition();
        Vector3 GetForwarding();
        bool GetState();
    }
}
