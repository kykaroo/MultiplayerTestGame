using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects.WeaponParts
{
    public class StockWeaponPartScriptableObject : WeaponPartScriptableObject
    {
        [SerializeField] private float spreadMultiplier = 1f;

        public float SpreadMultiplier => spreadMultiplier;
    }
}