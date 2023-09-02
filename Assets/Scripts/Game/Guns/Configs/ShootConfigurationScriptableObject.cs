using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Configuration", order = 2)]
    public class ShootConfigurationScriptableObject : ScriptableObject
    {
        [SerializeField] private bool isHitScan;
        [SerializeField] private string bulletPrefabPath;
        [SerializeField] private float bulletSpawnForce = 1000;
        [SerializeField] private float fireRate = 0.25f;
        [SerializeField] private float recoilRecoverySpeed = 1f;
        [SerializeField] private float maxSpreadTime = 1f;
        [SerializeField] private BulletSpreadType spreadType = BulletSpreadType.Simple;
        [Header("Simple Spread")]
        [SerializeField] private Vector3 spread = new(0.01f, 0.01f, 0.01f);

        [Header("Texture-Based spread")] [Range(0.001f, 5f)]
        [SerializeField] private float spreadMultiplier = 0.1f;
        [SerializeField] private Texture2D spreadTexture;
        
        [Header("Only for raycast (disabled)")]
        [SerializeField] private LayerMask hitMask;
        
        public bool IsHitScan => isHitScan;

        public string BulletPrefabPath => bulletPrefabPath;

        public float BulletSpawnForce => bulletSpawnForce;

        public float FireRate => fireRate;

        public float RecoilRecoverySpeed => recoilRecoverySpeed;

        public float MaxSpreadTime => maxSpreadTime;

        public BulletSpreadType SpreadType => spreadType;

        public Vector3 Spread => spread;

        public float SpreadMultiplier => spreadMultiplier;

        public Texture2D SpreadTexture => spreadTexture;

        public LayerMask HitMask => hitMask;
    }
}
