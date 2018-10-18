using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace SimLogistics.Display
{
    public class TransportDisplayer : MonoBehaviour
    {
        DisplayableBase displayer;
        bool isWorking = false;
        public void SetDisplayer(DisplayableBase _displayer)
        {
            displayer = _displayer;
            isWorking = true;
            transform.position = displayer.GetPosition();
            transform.LookAt(displayer.GetForwarding());
            //gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (isWorking)
            {
                transform.position = displayer.GetPosition();
                transform.LookAt(displayer.GetForwarding());
                if (!displayer.GetState())
                {
                    isWorking = false;
                }
            }
            else
            {
                if (displayer.GetState())
                {
                    isWorking = true;
                    transform.position = displayer.GetPosition();
                    transform.LookAt(displayer.GetForwarding());
                }
            }
        }
    }

}
