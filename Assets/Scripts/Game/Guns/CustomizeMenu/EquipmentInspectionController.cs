using UnityEngine;

namespace Game.Guns.CustomizeMenu
{
    public class EquipmentInspectionController : MonoBehaviour
    {
        private Transform _equipmentTransform;
        private Vector3 _lastMousePosition;
        private float _sensitivity = 10f;
        private float _scrollWheelInput;
        private Vector3 _originalEquipmentScale;
        private float _originalEquipmentScaleMaxX;
        private float _originalEquipmentScaleMinX;


        private void Awake()
        {
            _equipmentTransform = transform;
            _originalEquipmentScale = _equipmentTransform.localScale;
            _originalEquipmentScaleMaxX = _originalEquipmentScale.x * 1.5f;
            _originalEquipmentScaleMinX = _originalEquipmentScale.x * 0.75f;
        }

        private void Update()
        {
            Rotate();

            switch (Input.mouseScrollDelta.y)
            {
                case > 0:
                    ZoomIn();
                    break;
                case < 0:
                    ZoomOut();
                    break;
            }
        }

        private void ZoomOut()
        {
            if (_equipmentTransform.localScale.x < _originalEquipmentScaleMinX) return;
            
            _scrollWheelInput = Input.mouseScrollDelta.y * _sensitivity;
            _equipmentTransform.localScale += new Vector3(_scrollWheelInput, _scrollWheelInput, _scrollWheelInput);
        }

        private void ZoomIn()
        {
            if (_equipmentTransform.localScale.x > _originalEquipmentScaleMaxX) return;
            
            _scrollWheelInput = Input.mouseScrollDelta.y * _sensitivity;
            _equipmentTransform.localScale += new Vector3(_scrollWheelInput, _scrollWheelInput, _scrollWheelInput);
        }

        private void Rotate()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Vector3 mouseDelta = _lastMousePosition - Input.mousePosition;

                mouseDelta.x = Mathf.Clamp(mouseDelta.x, -200, +200);
                mouseDelta.y = Mathf.Clamp(mouseDelta.y, -200, +200);

                float rotateSpeed = 0.2f;

                var localEulerAngles = _equipmentTransform.localEulerAngles;
                localEulerAngles += new Vector3(mouseDelta.y, mouseDelta.x, 0f) * rotateSpeed;
                _equipmentTransform.localEulerAngles = localEulerAngles;

                float rotationXMin = -7f;
                float rotationXMax = 10f;

                float localEulerAnglesX = localEulerAngles.x;

                if (localEulerAnglesX > 180)
                {
                    localEulerAnglesX -= 360f;
                }

                float rotationX = Mathf.Clamp(localEulerAnglesX, rotationXMin, rotationXMax);

                var eulerAngles = _equipmentTransform.localEulerAngles;
                eulerAngles = new Vector3(rotationX, eulerAngles.y,
                    eulerAngles.x);
                _equipmentTransform.localEulerAngles = eulerAngles;
            }

            _lastMousePosition = Input.mousePosition;
        }
    }
}