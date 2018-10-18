using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace SimLogistics.Base
{
    using Common;

    [Serializable]
    public enum TransportationType
    {
        pickup = 0,
        truck = 1,
        helicopter = 2,
    }

    public enum TransportationState
    {
        carrying = 1,
        returning = 2,
        pioneering = 3,
        waiting = 0,
    }

    /// <summary>
    /// A struct to describe a real point in map
    /// It contains a Vector2 for it's coordinate, and a string name for its name
    /// </summary>
    [Serializable]
    public struct MapPoint
    {
        public string name;
        public Vector2 actualPosition;
        /// <summary>
        /// If name was not specified, it will be default empty string.
        /// </summary>
        /// <param name="pos">Actual position</param>
        public MapPoint(Vector2 pos)
        {
            name = "";
            actualPosition = pos;
        }
        public MapPoint(string _name, Vector2 _actualPosition)
        {
            name = _name;
            actualPosition = _actualPosition;
        }
    }

    /// <summary>
    /// Data structure to represent demand point
    /// </summary>
    [Serializable]
    public struct DP
    {
        /// <summary>
        /// An index, it's the position vector's index in map's Points collection
        /// </summary>
        public int position;
        public float demand;
    }
    [Serializable]
    public struct SC
    {
        public int position;
        public float reservation;
        public int[] numTransportations;
    }
    [Serializable]
    public struct Task
    {
        public int sc;
        public int dp;
        public float amount;
        public float time;
        public TransportationType transportationType;
        public ushort[] path;
    }

}
