using System;
using System.Collections.Generic;
using Game.Guns.CustomizeMenu.ScriptableObjects;
using UnityEngine;

namespace Game.Guns.CustomizeMenu
{
    public class WeaponBody : MonoBehaviour
    {
        [Serializable]
        public class PartTypeAttachPoint
        {
            public WeaponPartScriptableObject.PartType partType;
            public Transform attachPointTransform;
        }

        [SerializeField] private WeaponBodyScriptableObject _weaponBodyScriptableObject;
        [SerializeField] private List<PartTypeAttachPoint> _partTypeAttachPointsList;

        public WeaponBodyScriptableObject GetWeaponBodyScriptableObject()
        {
            return _weaponBodyScriptableObject;
        }

        public List<PartTypeAttachPoint> GetPartTypeAttachPointsList()
        {
            return _partTypeAttachPointsList;
        }
    }
}