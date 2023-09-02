using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects.WeaponParts
{
    public class MagazineWeaponPartScriptableObject : WeaponPartScriptableObject
    {
        [SerializeField] private int magazineCapacity = 20;
        [SerializeField] private float reloadTimeMultiplier = 1f;
    }
}