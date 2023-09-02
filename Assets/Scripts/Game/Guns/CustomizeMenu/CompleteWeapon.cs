using System;
using System.Collections.Generic;
using Game.Guns.CustomizeMenu.ScriptableObjects;
using Game.Guns.CustomizeMenu.ScriptableObjects.WeaponParts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Guns.CustomizeMenu
{
    public class CompleteWeapon : MonoBehaviour
    {
        public class AttachedWeaponPart
        {
            public WeaponBody.PartTypeAttachPoint PartTypeAttachPoint;
            public WeaponPartScriptableObject WeaponPartScriptableObject;
            public Transform SpawnedTransform;
        }

        [SerializeField] private List<WeaponPartScriptableObject> defaultWeaponPartScriptableObjectsList;

        private WeaponBody _weaponBody;
        private Dictionary<WeaponPartScriptableObject.PartType, AttachedWeaponPart> _attachedWeaponPartsDictionary;

        private void Awake()
        {
            _weaponBody = GetComponent<WeaponBody>();
            _attachedWeaponPartsDictionary = new();

            foreach (var partTypeAttachPoint in _weaponBody.GetPartTypeAttachPointsList())
            {
                _attachedWeaponPartsDictionary[partTypeAttachPoint.partType] = new()
                {
                    PartTypeAttachPoint = partTypeAttachPoint
                };
            }

            foreach (var weaponPartScriptableObject in defaultWeaponPartScriptableObjectsList)
            {
                SetPart(weaponPartScriptableObject);
            }
        }

        public void SetPart(WeaponPartScriptableObject weaponPartScriptableObject)
        {
            if (_attachedWeaponPartsDictionary[weaponPartScriptableObject.partType].SpawnedTransform != null)
            {
                Destroy(_attachedWeaponPartsDictionary[weaponPartScriptableObject.partType].SpawnedTransform.gameObject);
            }
            
            AttachedWeaponPart attachedWeaponPart = _attachedWeaponPartsDictionary[weaponPartScriptableObject.partType];
            Transform attachPointTransform = attachedWeaponPart.PartTypeAttachPoint.attachPointTransform;
            Transform spawnedPartTransform = Instantiate(weaponPartScriptableObject.prefab, attachPointTransform, false);
            attachedWeaponPart.SpawnedTransform = spawnedPartTransform;
            // Префабы модификаций отвечают за свой поворот и размер, но не место стыковки
            spawnedPartTransform.localPosition = Vector3.zero;

            attachedWeaponPart.WeaponPartScriptableObject = weaponPartScriptableObject;

            _attachedWeaponPartsDictionary[weaponPartScriptableObject.partType] = attachedWeaponPart;

            if (weaponPartScriptableObject.partType == WeaponPartScriptableObject.PartType.Barrel)
            {
                BarrelWeaponPartScriptableObject barrelWeaponPartScriptableObject =
                    (BarrelWeaponPartScriptableObject)weaponPartScriptableObject;

                AttachedWeaponPart barrelPartTypeAttachedWeaponPart =
                    _attachedWeaponPartsDictionary[WeaponPartScriptableObject.PartType.Barrel];
                AttachedWeaponPart muzzlePartTypeAttachedWeaponPart =
                    _attachedWeaponPartsDictionary[WeaponPartScriptableObject.PartType.Muzzle];

                muzzlePartTypeAttachedWeaponPart.PartTypeAttachPoint.attachPointTransform.position =
                    barrelPartTypeAttachedWeaponPart.PartTypeAttachPoint.attachPointTransform.position +
                    barrelPartTypeAttachedWeaponPart.PartTypeAttachPoint.attachPointTransform.forward *
                    barrelWeaponPartScriptableObject.muzzleOffset;
            }
        }

        public WeaponPartScriptableObject GetWeaponPartScriptableObject(WeaponPartScriptableObject.PartType partType)
        {
            AttachedWeaponPart attachedWeaponPart = _attachedWeaponPartsDictionary[partType];
            return attachedWeaponPart.WeaponPartScriptableObject;
        }

        public List<WeaponPartScriptableObject.PartType> GetWeaponPartTypeList()
        {
            return new(_attachedWeaponPartsDictionary.Keys);
        }

        public WeaponBodyScriptableObject GetWeaponBodyScriptableObject()
        {
            return _weaponBody.GetWeaponBodyScriptableObject();
        }
    }
}