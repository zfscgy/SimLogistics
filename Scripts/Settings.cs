using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimLogistics.Base
{
    public static class Settings
    {
        public static readonly int nKindTransports = 3;
        /// <summary>
        /// The game time * timeScale = the real time
        /// </summary>
        public static readonly float timeScale = 1000f;
        public static readonly float timeStep = 0.01f;
        /// <summary>
        /// The game distance * distanceScale = the real distance measured in m
        /// </summary>
        public static readonly float distanceScale = 1000f;
    }
}
