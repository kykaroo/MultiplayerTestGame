using System.Collections;
using Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.ItemSystem.NewSystem
{
    [CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
    public class GunScriptableObject : ScriptableObject
    {
        // public ImpactType ImpactType;
        public GunType Type;
        public string Name;
        public GameObject ModelPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;

        public ShootConfigurationScriptableObject ShootConfig;
        public TrailConfigurationScriptableObject TrailConfig;

        private MonoBehaviour ActiveMonoBehaviour;
        private GameObject Model;
        public PhotonView _photonView;

        private float LastShootTime;
        private float InitialClickTime;
        private float StopShootingTime;
        private bool LastFrameWantedToShoot;

        private ParticleSystem ShootSystem;
        private ObjectPool<Bullet> BulletPool;
        private ObjectPool<TrailRenderer> TrailPool;

        public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
        {
            ActiveMonoBehaviour = activeMonoBehaviour;
            LastShootTime = 0; // В эдиторе может некорректно отработать, в билде все норм
            TrailPool = new(CreateTrail);

            if (!ShootConfig.IsHitScan)
            {
                BulletPool = new ObjectPool<Bullet>(CreateBullet);
            }

            Model = PhotonNetwork.Instantiate("Items/NewItemSystem/Prefabs/M4A4", parent.position, Quaternion.identity);
            Model.transform.SetParent(parent, false);
            Model.transform.localPosition = SpawnPoint;
            Model.transform.localRotation = Quaternion.Euler(SpawnRotation);
            _photonView = Model.GetComponent<PhotonView>();
            ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
        }

        private TrailRenderer CreateTrail()
        {
            GameObject instance = new("Bullet Trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = TrailConfig.Color;
            trail.material = TrailConfig.Material;
            trail.widthCurve = TrailConfig.WidthCurve;
            trail.time = TrailConfig.Duration;
            trail.minVertexDistance = TrailConfig.MinVertexDistance;

            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }

        public void Tick(bool wantToShoot)
        {
            if (_photonView == null) return;
        
            if (!_photonView.IsMine) return;
        
            Model.transform.localRotation = Quaternion.Lerp(Model.transform.localRotation, Quaternion.Euler(SpawnRotation), Time.deltaTime * ShootConfig.RecoilRecoverySpeed);
        
            if (wantToShoot)
            {
                LastFrameWantedToShoot = true;
                Shoot();
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
            Bullet bullet = BulletPool.Get();
            bullet.gameObject.SetActive(true);
            bullet.OnCollision += HandleBulletCollision;
            bullet.transform.position = ShootSystem.transform.position;
            bullet.Spawn(shootDirection* ShootConfig.BulletSpawnForce);

            TrailRenderer trail = TrailPool.Get();
        
            if (trail != null)
            {
                trail.transform.SetParent(bullet.transform, false);
                trail.transform.localPosition = Vector3.zero;
                trail.emitting = true;
                trail.gameObject.SetActive(true);
            }
        }

        private void HandleBulletCollision(Bullet bullet, Collision collision)
        {
            TrailRenderer trail = bullet.GetComponent<TrailRenderer>();

            if (trail != null)
            {
                trail.transform.SetParent(null, true);
                ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
            }
        
            bullet.gameObject.SetActive(false);
            BulletPool.Release(bullet);

            if (collision != null)
            {
                ContactPoint contactPoint = collision.GetContact(0);
            
                HandleBulletImpact(Vector3.Distance(contactPoint.point, bullet.SpawnLocation), contactPoint.point, contactPoint.normal, contactPoint.otherCollider);
            }
        }

        private void HandleBulletImpact(float distanceTraveled, Vector3 hitLocation, Vector3 hitNormal, Collider hitCollider)
        {
            // TODO SurfaceManager

            // if (hitCollider.TryGetComponent(out IDamagable damagable))
            // {
            //     // TODO Новый IDamagable
            // }
        }

        private void DoHitscanShoot(Vector3 shootDirection)
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
        }

        private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
        {
            TrailRenderer instance = TrailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = startPoint;
            yield return null; // Предотвращение переноса позиции с прошлого кадра

            instance.emitting = true;

            float distance = Vector3.Distance(startPoint, endPoint);
            float remainingDistance = distance;

            while (remainingDistance > 0)
            {
                instance.transform.position =
                    Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - remainingDistance / distance));
                remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

                yield return null;
            }

            instance.transform.position = endPoint;

            if (hit.collider !=null)
            {
                HandleBulletImpact(distance, endPoint, hit.normal, hit.collider);
            }

            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            TrailPool.Release(instance);
        }

        private IEnumerator DelayedDisableTrail(TrailRenderer trail)
        {
            yield return new WaitForSeconds(TrailConfig.Duration);
            yield return null;
            trail.emitting = false;
            trail.gameObject.SetActive(false);
            TrailPool.Release(trail);
        }
    
        private Bullet CreateBullet()
        {
            return Instantiate(ShootConfig.BulletPrefab);
        }
    }
}
