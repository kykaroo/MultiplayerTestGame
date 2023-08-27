using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Customize/Weapon Body", fileName = "Weapon Body")]
    public class WeaponBodyScriptableObject : ScriptableObject
    {
        public enum Body
        {
            M4A4
        }

        public Body body;
        public Transform prefab;
        public Transform prefabUi;
        public WeaponPartListScriptableObject weaponPartListScriptableObject;
    }
}