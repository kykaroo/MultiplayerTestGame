using UnityEngine;

[CreateAssetMenu(menuName = "Customize/Weapon Part Type", fileName = "Weapon Part Type")]
public class WeaponPartScriptableObject : ScriptableObject
{
    public enum PartType
    {
        Barrel,
        Muzzle,
        Underbarrel,
        Stock,
        Grip,
        Scope,
        Magazine,
        Ammunition
    }

    public PartType partType;
    public Transform prefab;
}