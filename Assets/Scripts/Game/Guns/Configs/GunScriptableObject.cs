using System;
using System.Collections;
using Game.Guns.Modifiers.ProjectileCollisionEffects;
using Game.Player;
using Game.Player.PlayerInterfaces;
using Game.SurfaceManager;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject, ICloneable
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
        public TrailConfigurationScriptableObject TrailConfig;
        public AmmoConfigurationScriptableObject AmmoConfig;

        public ICollisionHandler[] bulletImpactEffects = Array.Empty<ICollisionHandler>();

        private MonoBehaviour ActiveMonoBehaviour;
        private GameObject Model;
        private PhotonView _photonView;

        private float LastShootTime;
        private float InitialClickTime;
        private float StopShootingTime;
        private bool LastFrameWantedToShoot;

        private ParticleSystem ShootSystem;

        private Transform _gunParent;

        public event Action<int, int> OnAmmunitionUpdate;

        public void EntryPoint(string modelPrefabPath, MonoBehaviour activeMonoBehaviour, ItemHolder itemHolder)
        {
            ActiveMonoBehaviour = activeMonoBehaviour;

            Model = itemHolder.CreateGun(modelPrefabPath, SpawnPoint, SpawnRotation);
            var gunSpawner = Model.GetComponent<GunSpawner>();
            // gunSpawner.Initialize(SpawnPoint, SpawnRotation);
            ShootSystem = gunSpawner.ShootSystem;
        }

        /*private TrailRenderer CreateTrail()
        {
            TrailRenderer trail = PhotonNetwork.Instantiate(TrailPrefabPath, Vector3.zero, Quaternion.identity).GetComponent<TrailRenderer>();
            trail.colorGradient = TrailConfig.Color;
            trail.material = TrailConfig.Material;
            trail.widthCurve = TrailConfig.WidthCurve;
            trail.time = TrailConfig.Duration;
            trail.minVertexDistance = TrailConfig.MinVertexDistance;
            trail.emitting = true;

            return trail;
        }*/

        public void Tick(bool wantToShoot)
        {
            Model.transform.localRotation = Quaternion.Lerp(Model.transform.localRotation, Quaternion.Euler(SpawnRotation), Time.deltaTime * ShootConfig.RecoilRecoverySpeed);
        
            if (wantToShoot)
            {
                LastFrameWantedToShoot = true;
                if (AmmoConfig.currentClipAmmo > 0)
                { 
                    Shoot();
                }
            }
            else
            {
                if (!wantToShoot && LastFrameWantedToShoot)
                {
                    StopShootingTime = Time.time;
                    LastFrameWantedToShoot = false;
                }
            }
        }
        public void Shoot()
        {
            if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
            {
                float lastDuration = Mathf.Clamp(0, StopShootingTime - InitialClickTime, ShootConfig.MaxSpreadTime);
                float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTime)) / ShootConfig.RecoilRecoverySpeed;
                InitialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            }
        
            if (Time.time > ShootConfig.FireRate + LastShootTime)
            {
                LastShootTime = Time.time;
            
                Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialClickTime);
                Model.transform.forward += Model.transform.TransformDirection(spreadAmount);
            
                Vector3 shootDirection = Model.transform.forward;
            
                ShootSystem.Play();

                AmmoConfig.currentClipAmmo--;
                OnAmmunitionUpdate?.Invoke(AmmoConfig.currentClipAmmo, AmmoConfig.currentAmmo);

                if (ShootConfig.IsHitScan)
                {
                    DoHitscanShoot(shootDirection);
                }
                else
                {
                    DoProjectileShoot(shootDirection);
                }
            }
        }
        
        private void DoProjectileShoot(Vector3 shootDirection)
        {
            Bullet bullet = PhotonNetwork.Instantiate(ShootConfig.bulletPrefabPath, ShootSystem.transform.position, Quaternion.identity)
                .GetComponent<Bullet>();
            bullet.OnCollision += HandleBulletCollision;
            bullet.Spawn(shootDirection * ShootConfig.BulletSpawnForce);

            /*TrailRenderer trail = CreateTrail();
        
            if (trail != null)
            {
                trail.transform.SetParent(bullet.transform, false);
                trail.transform.localPosition = Vector3.zero;
            }*/
        }

        private void HandleBulletCollision(Bullet bullet, Collision collision)
        {
            // TrailRenderer trail = bullet.GetComponent<TrailRenderer>();

            // if (trail != null)
            // {
            //     trail.transform.SetParent(null, true);
            //     ActiveMonoBehaviour.StartCoroutine(DelayedDestroyTrail(trail));
            // }

            PhotonNetwork.Destroy(bullet.gameObject);

            if (collision != null)
            {
                ContactPoint contactPoint = collision.GetContact(0);
            
                HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.SpawnLocation), contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
            }
        }

        private void HandleBulletImpact(float distanceTraveled, Vector3 hitLocation, Vector3 hitNormal, Collider hitCollider)
        {
            SurfaceManager.SurfaceManager.Instance.HandleImpact(hitCollider.gameObject, hitLocation, hitNormal, ImpactType, 0);

            if (hitCollider.transform.root.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(distanceTraveled), hitCollider.name);
            }
            
            foreach (var handler in bulletImpactEffects)
            {
                handler.HandleImpact(hitCollider, hitLocation, hitNormal, this);
            }
        }

        private void DoHitscanShoot(Vector3 shootDirection)
        {
            // if (Physics.Raycast(ShootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue,
            //         ShootConfig.HitMask))
            // {
            //     ActiveMonoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position, hit.point, hit));
            // }
            // else
            // {
            //     ActiveMonoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position,
            //         ShootSystem.transform.position + shootDirection * TrailConfig.MissDistance, new RaycastHit()));
            // }
        }

        /*private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
        {
            TrailRenderer trail = CreateTrail();
            trail.transform.position = startPoint;
            yield return null; // Предотвращение переноса позиции с прошлого кадра

            trail.emitting = true;

            float distance = Vector3.Distance(startPoint, endPoint);
            float remainingDistance = distance;

            while (remainingDistance > 0)
            {
                trail.transform.position =
                    Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - remainingDistance / distance));
                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            trail.transform.position = endPoint;

            if (hit.collider !=null)
            {
                HandleBulletImpact(distance, endPoint, hit.normal, hit.collider);
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;

            PhotonNetwork.Destroy(trail.gameObject);
        }*/

        private IEnumerator DelayedDestroyTrail(TrailRenderer trail)
        {
            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            PhotonNetwork.Destroy(trail.gameObject);
        }
        
        public bool CanReload()
        {
            return AmmoConfig.CanReload();
        }
        
        public void EndReload()
        { 
            AmmoConfig.Reload();
        }

        public object Clone()
        {
            GunScriptableObject config = CreateInstance<GunScriptableObject>();
            config.Type = Type;
            config.Name = Name;
            config.name = name;
            config.ImpactType = ImpactType;
            config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
            config.ShootConfig = ShootConfig.Clone() as ShootConfigurationScriptableObject;
            config.AmmoConfig = AmmoConfig.Clone() as AmmoConfigurationScriptableObject;
            config.TrailConfig = TrailConfig.Clone() as TrailConfigurationScriptableObject;

            config.ModelPrefab = ModelPrefab;
            config.SpawnPoint = SpawnPoint;
            config.SpawnRotation = SpawnRotation;

            return config;
        }
    }
}
