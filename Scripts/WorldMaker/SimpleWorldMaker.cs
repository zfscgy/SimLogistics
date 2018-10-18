using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimLogistics.WorldMaker
{
    using Base;
    using Common;
    public class SimpleWorldMaker: MonoBehaviour
    {
        public Transform[] Points;
        public SC supplyCenter;
        public DP[] DemandPoints;
        public List<string> PathStrings;

        public Map Map { get; private set; }
        public void GenerateMap()
        {
            List<Vector2> points = new List<Vector2>();
            foreach(Transform p in Points)
            {
                points.Add(new Vector2(p.position.x, p.position.z));
            }
            List<List<ushort>> paths = new List<List<ushort>>();
            foreach(string s in PathStrings)
            {
                List<ushort> nums = new List<ushort>();
                string[] numStrings = s.Split(' ');
                foreach(string numStr in numStrings)
                {
                    nums.Add(ushort.Parse(numStr));
                }
                paths.Add(nums);
            }
            Map = new Map(points.ToArray(), paths);
        }
    }
}
