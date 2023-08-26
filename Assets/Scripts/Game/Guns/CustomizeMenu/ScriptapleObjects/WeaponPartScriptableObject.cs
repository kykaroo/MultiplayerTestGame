using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Part Type", fileName = "Customize/Weapon Part Type")]
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
        Magazine
    }

    public PartType partType;
    public Transform prefab;
}