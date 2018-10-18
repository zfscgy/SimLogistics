using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace SimLogistics.Control
{
    public class InputManager : MonoBehaviour
    {
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public float KeyX { get; private set; }
        public float KeyY { get; private set; }
        private void Update()
        {
            MouseX = - Input.GetAxis("Mouse Y");
            MouseY = Input.GetAxis("Mouse X");
            KeyX = (Input.GetKey(KeyCode.A) ? 0f : - 1f) + (Input.GetKey(KeyCode.D) ? 0f : 1f);
            KeyY = (Input.GetKey(KeyCode.W) ? 0f : 1f) + (Input.GetKey(KeyCode.S) ? 0f : -1f);
        }

    }
}
