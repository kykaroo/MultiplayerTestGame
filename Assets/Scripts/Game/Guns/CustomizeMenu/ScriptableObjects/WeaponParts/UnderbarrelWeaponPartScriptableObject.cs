using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects.WeaponParts
{
    public class UnderbarrelWeaponPartScriptableObject : WeaponPartScriptableObject
    {
        [SerializeField] private float spreadMultiplier = 1f;

        public float SpreadMultiplier => spreadMultiplier;
    }
}