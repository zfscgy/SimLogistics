using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace SimLogistics.Display
{
    using Control;
    public class CameraController : MonoBehaviour
    {
        public InputManager inputManager;
        public float rotationSpeed;
        public float movingSpeed;


        void Update()
        {
            float angVelocityY = inputManager.MouseY * rotationSpeed * Time.deltaTime;
            float angVelocityX = inputManager.MouseX * rotationSpeed * Time.deltaTime;
            transform.eulerAngles += new Vector3(angVelocityX, angVelocityY, 0);
            transform.position += - movingSpeed * inputManager.KeyX * Time.deltaTime * transform.right -
                movingSpeed * inputManager.KeyY * Time.deltaTime * transform.forward;
        }
    }
}
