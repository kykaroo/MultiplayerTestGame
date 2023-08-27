using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Customize/Weapon Barrel Part", fileName = "Weapon Barrel Part")]
    public class BarrelWeaponPartScriptableObject : WeaponPartScriptableObject
    {
        public float muzzleOffset;
    }
}