using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Customize/Weapon Body List", fileName = "Weapon Body List")]
    public class WeaponBodyListScriptableObjects : ScriptableObject
    {
        public WeaponBodyScriptableObject m4A4BodyScriptableObject;
    }
}