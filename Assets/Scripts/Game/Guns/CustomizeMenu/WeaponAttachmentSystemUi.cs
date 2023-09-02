using Game.Guns.CustomizeMenu.ScriptableObjects;
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
            AddListenersToDropdowns();
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
        
        private void AddListenersToDropdowns()
        {
            muzzleDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Muzzle, partIndex));
            barrelDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Barrel, partIndex));
            underbarrelDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Underbarrel, partIndex));
            stockDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Stock, partIndex));
            gripDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Grip, partIndex));
            scopeDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Scope, partIndex));
            magazineDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Magazine, partIndex));
            ammunitionDropdown.onValueChanged.AddListener(partIndex =>
                WeaponAttachmentSystem.Instance.ChangePart(WeaponPartScriptableObject.PartType.Ammunition, partIndex));
        }
    }
}