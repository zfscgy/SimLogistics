using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimLogistics.Display
{
    using Common;
    using Base;
    public class DisplayController : MonoBehaviour
    {
        public GameObject[] TransportationPrefabs;
        private List<Transportation> Transportations;
        private Dictionary<Transportation, TransportDisplayer> TransportDisplayers = new Dictionary<Transportation, TransportDisplayer>();

        public void Init(IEnumerable<Transportation> transportations)
        {
            Transportations = new List<Transportation>(transportations);
            foreach(Transportation tp in Transportations)
            {
                TransportDisplayer transportDisplayer = Instantiate(TransportationPrefabs[(int)tp.Type]).GetComponent<TransportDisplayer>();
                transportDisplayer.SetDisplayer(tp);
                TransportDisplayers.Add(tp, transportDisplayer);
            }
        }
    }
}
