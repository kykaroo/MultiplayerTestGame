using System.Collections;
using Game.Guns.Configs;
using Game.Player;
using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public abstract class AreaOfEffect : ICollisionHandler
    {
        public float AreaRadius;
        public AnimationCurve DamageFallOff;
        public int BaseDamage;
        
        private LayerMask hitMask;
        private Collider[] _hitObjects = new Collider[100];
        protected int Hits;

        public AreaOfEffect(float areaRadius, AnimationCurve damageFallOff, int baseDamage)
        {
            AreaRadius = areaRadius;
            DamageFallOff = damageFallOff;
            BaseDamage = baseDamage;
            hitMask = LayerMask.GetMask("AreaOfEffectCollider", "DestructibleObjects");
        }

        public virtual void HandleImpact(Collider impactedObject, Vector3 hitPosition, Vector3 hitNormal, GunScriptableObject gun)
        {
            Hits = Physics.OverlapSphereNonAlloc(hitPosition, AreaRadius, _hitObjects,
                hitMask);

            for (int i = 0; i < Hits; i++)
            {
                if (_hitObjects[i].transform.root.TryGetComponent(out IDamageable damageable))
                {
                    float distance = Vector3.Distance(hitPosition, _hitObjects[i].ClosestPoint(hitPosition));
                    
                    damageable.TakeDamage(Mathf.CeilToInt(BaseDamage * DamageFallOff.Evaluate(distance / AreaRadius)), default);
                }
            }
        }
    }
}