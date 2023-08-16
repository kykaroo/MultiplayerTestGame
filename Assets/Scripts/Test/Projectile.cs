using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile<T> : MonoBehaviour where T : ItemInfo
{
    public ItemInfo projectileInfo;
    public GameObject projectileImpactPrefab;
    public float damage;
}
