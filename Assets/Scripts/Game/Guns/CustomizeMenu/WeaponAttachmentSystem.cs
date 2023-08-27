using System;
using System.Collections.Generic;
using Game.Guns.CustomizeMenu.ScriptableObjects;
using UnityEngine;

namespace Game.Guns.CustomizeMenu
{
    public class WeaponAttachmentSystem : MonoBehaviour
    {
        public static WeaponAttachmentSystem Instance { get; private set; }
        
        [SerializeField] private WeaponBodyListScriptableObjects weaponBodyListScriptableObjects;
        [SerializeField] private WeaponBodyScriptableObject weaponBodyScriptableObject;
        [SerializeField] private Transform uiWindow;

        private CompleteWeapon _completeWeapon;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetWeaponBody(weaponBodyScriptableObject);
        }

        private void SetWeaponBody(WeaponBodyScriptableObject weaponBodyScriptableObject)
        {
            Vector3 previousEulerAngles = Vector3.zero;

            if (_completeWeapon != null)
            {
                previousEulerAngles = _completeWeapon.transform.eulerAngles;
                Destroy(_completeWeapon.gameObject);
            }

            previousEulerAngles = weaponBodyScriptableObject.prefab.eulerAngles;

            weaponBodyScriptableObject = this.weaponBodyScriptableObject;

            Transform weaponBodyTransform = Instantiate(weaponBodyScriptableObject.prefab, uiWindow, false);
            weaponBodyTransform.eulerAngles = previousEulerAngles;
            _completeWeapon = weaponBodyTransform.GetComponent<CompleteWeapon>();

            Instantiate(weaponBodyScriptableObject.prefabUi, uiWindow, false);
        }

        public void ChangePart(WeaponPartScriptableObject.PartType partType, int partIndex)
        {
            WeaponPartScriptableObject equippedWeaponPartScriptableObject =
                _completeWeapon.GetWeaponPartScriptableObject(partType);

            if (equippedWeaponPartScriptableObject == null)
            {
                _completeWeapon.SetPart(weaponBodyScriptableObject.weaponPartListScriptableObject
                    .GetWeaponPartScriptableObjectList(partType)[0]);
            }
            else
            {
                List<WeaponPartScriptableObject> weaponPartScriptableObjectsList = weaponBodyScriptableObject
                    .weaponPartListScriptableObject.GetWeaponPartScriptableObjectList(partType);
                _completeWeapon.SetPart(weaponPartScriptableObjectsList[partIndex]);
            }
        }
    }
}