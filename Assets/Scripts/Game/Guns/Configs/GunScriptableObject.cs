using Game.SurfaceManager;
using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject
    {
        [SerializeField] private ImpactType impactType;
        [SerializeField] private GunType type;
        [SerializeField] private string prefabPath;
        [SerializeField] private string weaponName;
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private Vector3 spawnPoint;
        [SerializeField] private Vector3 spawnRotation;

        [SerializeField] private DamageConfigScriptableObject damageConfig;
        [SerializeField] private ShootConfigurationScriptableObject shootConfig;
        [SerializeField] private AmmoConfigurationScriptableObject ammoConfig;

        public ImpactType ImpactType => impactType;

        public GunType Type => type;

        public string WeaponName => weaponName;

        public GameObject ModelPrefab => modelPrefab;

        public Vector3 SpawnPoint => spawnPoint;

        public Vector3 SpawnRotation => spawnRotation;

        public DamageConfigScriptableObject DamageConfig => damageConfig;

        public ShootConfigurationScriptableObject ShootConfig => shootConfig;

        public AmmoConfigurationScriptableObject AmmoConfig => ammoConfig;
        
        public string PrefabPath => prefabPath;
    }
}
