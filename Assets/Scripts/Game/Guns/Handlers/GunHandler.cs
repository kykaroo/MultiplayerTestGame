using System;
using Game.Guns.Configs;
using Game.Guns.Modifiers.ProjectileCollisionEffects;
using Game.Player;
using Game.Player.PlayerInterfaces;
using Game.SurfaceManager;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Handlers
{
    public class GunHandler
    {
        public ImpactType impactType;
        public GunType type;
        public string gunName;
        public Vector3 spawnPoint;
        public Vector3 spawnRotation;
        
        private MonoBehaviour _activeMonoBehaviour;
        private GameObject _model;

        private float _lastShootTime;
        private float _initialClickTime;
        private float _stopShootingTime;
        private bool _lastFrameWantedToShoot;

        private ParticleSystem _shootSystem;

        public DamageHandler DamageHandler;
        public ShootHandler ShootHandler;
        public AmmoHandler AmmoHandler;
        
        public ICollisionHandler[] BulletImpactEffects = Array.Empty<ICollisionHandler>();
        
        public event Action<int, int> OnAmmunitionUpdate;

        public GunHandler(GunScriptableObject gunScriptableObject, string modelPrefabPath, PlayerItemSelector activeMonoBehaviour, ItemHolder itemHolder)
        {
            _activeMonoBehaviour = activeMonoBehaviour;
            impactType = gunScriptableObject.ImpactType;
            type = gunScriptableObject.Type;
            gunName = gunScriptableObject.Name;
            spawnPoint = gunScriptableObject.SpawnPoint;
            spawnRotation = gunScriptableObject.SpawnRotation;

            _model = itemHolder.CreateGun(modelPrefabPath, spawnPoint, spawnRotation);
            _shootSystem = _model.GetComponent<GunPartsHandler>().ParticleSystem;

            DamageHandler = new(gunScriptableObject.DamageConfig);
            ShootHandler = new(gunScriptableObject.ShootConfig);
            AmmoHandler = new(gunScriptableObject.AmmoConfig);
        }
        public void Tick(bool wantToShoot)
        {
            _model.transform.localRotation = Quaternion.Lerp(_model.transform.localRotation,
                Quaternion.Euler(spawnRotation), Time.deltaTime * ShootHandler.RecoilRecoverySpeed);
        
            if (wantToShoot)
            {
                _lastFrameWantedToShoot = true;
                if (AmmoHandler.CurrentClipAmmo > 0)
                { 
                    Shoot();
                }
            }
            else
            {
                if (!wantToShoot && _lastFrameWantedToShoot)
                {
                    _stopShootingTime = Time.time;
                    _lastFrameWantedToShoot = false;
                }
            }
        }
        public void Shoot()
        {
            if (Time.time - _lastShootTime - ShootHandler.FireRate > Time.deltaTime)
            {
                float lastDuration = Mathf.Clamp(0, _stopShootingTime - _initialClickTime, ShootHandler.MaxSpreadTime);
                float lerpTime = (ShootHandler.RecoilRecoverySpeed - (Time.time - _stopShootingTime)) / ShootHandler.RecoilRecoverySpeed;
                _initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
            }
        
            if (Time.time > ShootHandler.FireRate + _lastShootTime)
            {
                _lastShootTime = Time.time;
            
                Vector3 spreadAmount = ShootHandler.GetSpread(Time.time - _initialClickTime);
                _model.transform.forward += _model.transform.TransformDirection(spreadAmount);
                _model.transform.forward = _model.transform.forward;

                Vector3 shootDirection = _model.transform.forward;
            
                _shootSystem.Play();

                AmmoHandler.CurrentClipAmmo--;
                OnAmmunitionUpdate?.Invoke(AmmoHandler.CurrentClipAmmo, AmmoHandler.CurrentAmmo);

                /*if (_shootHandler.IsHitScan)
                {
                    DoHitscanShoot(shootDirection);
                }
                else
                {
                    DoProjectileShoot(shootDirection);
                }*/
                DoProjectileShoot(shootDirection);
            }
        }
        
        private void DoProjectileShoot(Vector3 shootDirection)
        {
            Bullet bullet = PhotonNetwork.Instantiate(ShootHandler.BulletPrefabPath, _shootSystem.transform.position, Quaternion.identity)
                .GetComponent<Bullet>();
            bullet.OnCollision += HandleBulletCollision;
            bullet.Spawn(shootDirection * ShootHandler.BulletSpawnForce);

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
            SurfaceManager.SurfaceManager.Instance.HandleImpact(hitCollider.gameObject, hitLocation, hitNormal, impactType, 0);

            if (hitCollider.transform.root.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageHandler.GetDamage(distanceTraveled), hitCollider.name);
            }
            
            foreach (var handler in BulletImpactEffects)
            {
                handler.HandleImpact(hitCollider, hitLocation, hitNormal, this);
            }
        }
        
        public bool CanReload()
        {
            return AmmoHandler.CanReload();
        }
        
        public void EndReload()
        { 
            AmmoHandler.Reload();
        }

        /*private void DoHitscanShoot(Vector3 shootDirection)
        {
            if (Physics.Raycast(ShootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue,
                    ShootConfig.HitMask))
            {
                ActiveMonoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position, hit.point, hit));
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine(PlayTrail(ShootSystem.transform.position,
                    ShootSystem.transform.position + shootDirection * TrailConfig.MissDistance, new RaycastHit()));
            }
        }*/

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

        /*private IEnumerator DelayedDestroyTrail(TrailRenderer trail)
        {
            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            PhotonNetwork.Destroy(trail.gameObject);
        }*/

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
    }
}