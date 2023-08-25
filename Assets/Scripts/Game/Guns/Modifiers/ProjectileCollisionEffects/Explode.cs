using Game.Guns.Configs;
using Game.Player;
using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class Explode : ICollisionHandler
    {
        public float ExplosionRadius;
        public AnimationCurve DamageFallOff;
        public int BaseDamage;
        
        private LayerMask hitMask;
        private Collider[] _hitObjects = new Collider[100];

        public Explode(float explosionRadius, AnimationCurve damageFallOff, int baseDamage)
        {
            ExplosionRadius = explosionRadius;
            DamageFallOff = damageFallOff;
            BaseDamage = baseDamage;
            hitMask = LayerMask.GetMask("ExplosionCollider", "DistractibleObjects");
        }

        public void HandleImpact(Collider impactedObject, Vector3 hitPosition, Vector3 hitNormal, GunScriptableObject gun)
        {
            int hits = Physics.OverlapSphereNonAlloc(hitPosition, ExplosionRadius, _hitObjects,
                hitMask);

            for (int i = 0; i < hits; i++)
            {
                if (_hitObjects[i].transform.root.TryGetComponent(out IDamageable damageable))
                {
                    float distance = Vector3.Distance(hitPosition, _hitObjects[i].ClosestPoint(hitPosition));
                    
                    damageable.TakeDamage(Mathf.CeilToInt(BaseDamage * DamageFallOff.Evaluate(distance / ExplosionRadius)), default);
                }
            }
        }
    }
}