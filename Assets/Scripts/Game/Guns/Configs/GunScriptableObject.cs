using System;
using Game.Guns.Modifiers.ProjectileCollisionEffects;
using Game.SurfaceManager;
using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject
    {
        public ImpactType ImpactType;
        public GunType Type;
        public string Name;
        public GameObject ModelPrefab;
        public string TrailPrefabPath;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;

        public DamageConfigScriptableObject DamageConfig;
        public ShootConfigurationScriptableObject ShootConfig;
        // public TrailConfigurationScriptableObject TrailConfig;
        public AmmoConfigurationScriptableObject AmmoConfig;
    }
}
