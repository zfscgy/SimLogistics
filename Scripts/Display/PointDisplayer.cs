using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SimLogistics.Display
{
    public class PointDisplayer : MonoBehaviour
    {
        public TextMesh text;
        public void SetText(string s)
        {
            text.text = s;
        }
    }
}
