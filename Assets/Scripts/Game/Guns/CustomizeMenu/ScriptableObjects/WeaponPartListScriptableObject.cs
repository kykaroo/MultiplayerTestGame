using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Guns.CustomizeMenu.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Customize/Weapon Part List", fileName = "Weapon Part List")]
    public class WeaponPartListScriptableObject : ScriptableObject
    {
        public List<WeaponPartScriptableObject> weaponPartScriptableObjectsList;

        public List<WeaponPartScriptableObject> GetWeaponPartScriptableObjectList(
            WeaponPartScriptableObject.PartType partType)
        {
            return weaponPartScriptableObjectsList
                .Where(weaponPartScriptableObject => weaponPartScriptableObject.partType == partType).ToList();
        }
        
        public List<string> GetWeaponPartScriptableObjectsStringList(
            WeaponPartScriptableObject.PartType partType)
        {
            return (from weaponPartScriptableObject in weaponPartScriptableObjectsList
                where weaponPartScriptableObject.partType == partType
                select weaponPartScriptableObject.name).ToList();
        }
    }
}