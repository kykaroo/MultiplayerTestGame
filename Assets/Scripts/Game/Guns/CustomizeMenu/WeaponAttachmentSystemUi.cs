using Game.Guns.CustomizeMenu.ScriptapleObjects;
using TMPro;
using UnityEngine;

namespace Game.Guns.CustomizeMenu
{
    public class WeaponAttachmentSystemUi : MonoBehaviour
    {
        [SerializeField] private WeaponPartListScriptableObject weaponPartList;
        [SerializeField] private TMP_Dropdown muzzleDropdown;
        [SerializeField] private TMP_Dropdown barrelDropdown;
        [SerializeField] private TMP_Dropdown underbarrelDropdown;
        [SerializeField] private TMP_Dropdown stockDropdown;
        [SerializeField] private TMP_Dropdown gripDropdown;
        [SerializeField] private TMP_Dropdown scopeDropdown;
        [SerializeField] private TMP_Dropdown magazineDropdown;
        [SerializeField] private TMP_Dropdown ammunitionDropdown;

        private void Awake()
        {
            InitializeDropdowns();
        }

        private void InitializeDropdowns()
        {
            muzzleDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Muzzle));
            barrelDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Barrel));
            underbarrelDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Underbarrel));
            stockDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Stock));
            gripDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Grip));
            scopeDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Scope));
            magazineDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Magazine));
            ammunitionDropdown.AddOptions(
                weaponPartList.GetWeaponPartScriptableObjectsStringList(WeaponPartScriptableObject.PartType.Ammunition));
        }
    }
}