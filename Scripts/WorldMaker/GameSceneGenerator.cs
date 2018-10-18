using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimLogistics.WorldMaker
{
    using Base;
    using Common;
    using Display;
    public class GameSceneGenerator : MonoBehaviour
    {
        public GameObject pointPrefab;
        public Transform mapTransform;
        Map map;
        HeadQuater hq;

        List<GameObject> Points;
        List<GameObject> SupplyCenters;
        List<GameObject> demandPoints;
        public void GenerateScene()
        {
            for(int i = 0; i < map.NPoint(); i++)
            {
                MapPoint point = map.GetPoint(i);
                Vector2 point2dPos = point.actualPosition;
                PointDisplayer pointDisplayer = Instantiate(
                    pointPrefab, new Vector3(point2dPos.x, 0, point2dPos.y), 
                    Quaternion.identity, mapTransform).GetComponent<PointDisplayer>();
                pointDisplayer.SetText(i.ToString() + "_" + point.name);
            }
        }
    }
}
