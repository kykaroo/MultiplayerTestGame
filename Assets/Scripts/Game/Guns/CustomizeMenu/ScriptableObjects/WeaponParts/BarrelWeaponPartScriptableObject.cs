using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects.WeaponParts
{
    [CreateAssetMenu(menuName = "Customize/Weapon Barrel Part", fileName = "Weapon Barrel Part")]
    public class BarrelWeaponPartScriptableObject : WeaponPartScriptableObject
    {
        [SerializeField] private float muzzleOffset;

        public float MuzzleOffset => muzzleOffset;
    }
}