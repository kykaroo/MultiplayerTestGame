using System;
using UnityEngine;

namespace Game.Guns.GunCustomizeScreen
{
    public class EquipmentClickRotate : MonoBehaviour
    {
        public Transform equipmentTransform;
        private Vector3 _lastMousePosition;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector3 mouseDelta = _lastMousePosition - Input.mousePosition;

                mouseDelta.x = Mathf.Clamp(mouseDelta.x, -200, +200);
                mouseDelta.y = Mathf.Clamp(mouseDelta.y, -200, +200);

                float rotateSpeed = 0.2f;

                equipmentTransform.localEulerAngles += new Vector3(mouseDelta.y, mouseDelta.x, 0f) * rotateSpeed;

                float rotationXMin = -7f;
                float rotationXMax = 10f;

                float localEulerAnglesX = equipmentTransform.localEulerAngles.x;

                if (localEulerAnglesX > 180)
                {
                    localEulerAnglesX -= 360f;
                }

                float rotationX = Mathf.Clamp(localEulerAnglesX, rotationXMin, rotationXMax);

                equipmentTransform.localEulerAngles = new Vector3(rotationX, equipmentTransform.localEulerAngles.y,
                    equipmentTransform.localEulerAngles.x);
            }

            _lastMousePosition = Input.mousePosition;
        }
    }
}