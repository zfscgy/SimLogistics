using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine;

namespace SimLogistics.WorldMaker
{
    using Base;
    using Common;

    public class FileMapMaker
    {

    }

    public class FileWorldMaker
    {
        private List<Vector2> Points = new List<Vector2>();
        private List<List<ushort>> Routes = new List<List<ushort>>();
        private List<SC> SCs;
        private List<DP> DPs;

        Map map;
        HeadQuater headQuater;
        /// <summary>
        /// Make world from the file
        /// Assume the file format like this:
        /// Points:
        ///  point1_x point1_y
        ///  point2_x point2_y
        ///  ...
        /// Routes:
        ///  route_1_point_1 route_1_point_2 ...
        ///  route_2_point_1 route_2_point_2 ...
        /// ...
        /// Supply Centers:
        ///  sc_1_pos sc_1_reservation sc_1_transport_1_num sc_1_transport_2_num ...
        ///  sc_2_pos sc_2_reservation sc_2_transport_1_num ...
        /// Demand Points:
        ///  dp_1_pos dp_1_demand
        ///  dp_2_pos dp_2_demand
        ///  ...
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool ReadWorldFromFile(string filename)
        {
            StreamReader streamReader = File.OpenText(filename);
            string line = streamReader.ReadLine();
            if (line != "Points:") return false;
            while (line != "Routes:")
            {
                try
                {
                    string[] numStrs = line.Split(' ');
                    Points.Add(new Vector2(float.Parse(numStrs[0]), float.Parse(numStrs[1])) / Settings.distanceScale);
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            line = streamReader.ReadLine();
            while (line != "SupplyCenters:")
            {
                string[] routePoints = line.Split(' ');
                List<ushort> pathNodes = new List<ushort>();
                foreach (string s in routePoints)
                {
                    pathNodes.Add(ushort.Parse(s));
                }
                Routes.Add(pathNodes);
            }
            line = streamReader.ReadLine();
            while (line != "Demand Points:")
            {
                string[] scInfos = line.Split(' ');
                int pos = int.Parse(scInfos[0]);
                int reserv = int.Parse(scInfos[1]);
                int transportListLen = scInfos.Length - 2;
                int[] transportList = new int[transportListLen];
                for (int i = 0; i < transportListLen; i++)
                {
                    transportList[i] = int.Parse(scInfos[i - 2]);
                }
                SCs.Add(new SC { position = pos, reservation = reserv, numTransportations = transportList });
            }
            line = streamReader.ReadLine();
            while (line != null)
            {
                string[] dpInfos = line.Split(' ');
                DPs.Add(new DP { position = int.Parse(dpInfos[0]), demand = int.Parse(dpInfos[1]) });
            }
            streamReader.Close();
            return true;
        }
        public void Generate()
        {
            map = new Map(Points.ToArray(), Routes);
            headQuater = new HeadQuater(map, DPs.ToArray(), SCs.ToArray());
        }
    }
}
