using TMPro;
using UnityEngine;

namespace Game.ItemSystem
{
    [CreateAssetMenu(menuName = "FPS/New Gun")]
    public class WeaponConfig : ItemInfo
    {
        [SerializeField] private ProjectileConfig projectileType;
        [SerializeField] private float shootForce;
        [SerializeField] private float upwardForce;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private float spread;
        [SerializeField] private float reloadTime;
        [SerializeField] private float timeBetweenShoots;
        [SerializeField] private int bulletPerTap;
        [SerializeField] private int magazineSize;
        [SerializeField] private bool allowButtonHold;
        [SerializeField] private GameObject muzzleFlash;
        
        public ProjectileConfig ProjectileType => projectileType;

        public float ShootForce => shootForce;

        public float UpwardForce => upwardForce;

        public float TimeBetweenShooting => timeBetweenShooting;

        public float Spread => spread;

        public float ReloadTime => reloadTime;

        public float TimeBetweenShoots => timeBetweenShoots;

        public int BulletPerTap => bulletPerTap;

        public int MagazineSize => magazineSize;

        public bool AllowButtonHold => allowButtonHold;

        public GameObject MuzzleFlash => muzzleFlash;
    }
}
