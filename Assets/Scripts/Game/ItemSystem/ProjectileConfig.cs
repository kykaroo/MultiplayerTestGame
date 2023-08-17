using UnityEngine;
using UnityEngine.Serialization;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "FPS/New Projectile")]
    public class ProjectileConfig : ItemInfo
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private string projectilePrefabPath;
        [SerializeField] private GameObject impactPrefab;
        [SerializeField] private float damage;
        [SerializeField] private float explosionRadius;
        [SerializeField] private float explosionDamageFalloff; 
        [SerializeField] private float velocity;
        
        public GameObject ProjectilePrefab => projectilePrefab;

        public GameObject ImpactPrefab => impactPrefab;

        public float Damage => damage;

        public float ExplosionRadius => explosionRadius;

        public float ExplosionDamageFalloff => explosionDamageFalloff;

        public float Velocity => velocity;
        
        public string ProjectilePrefabPath => projectilePrefabPath;
    }
}
