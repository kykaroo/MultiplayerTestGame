using Game.Guns.Handlers;
using Game.Player.PlayerInterfaces;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class FrostModifier : AreaOfEffect
    {
        public float SlowPercent;
        public float SlowTime;
        public bool IsStackable;
        public int MaxStacks;

        public FrostModifier(float areaRadius, AnimationCurve damageFallOff, int baseDamage) : base(areaRadius,
            damageFallOff, baseDamage)
        {
            SlowPercent = new();
        }

        public FrostModifier(float areaRadius, AnimationCurve damageFallOff, int baseDamage, float slowPercent, float slowTime, bool isStackable, int maxStacks) :
            base(areaRadius, damageFallOff, baseDamage)
        {
            SlowPercent = slowPercent;
            SlowTime = slowTime;
            IsStackable = isStackable;
            MaxStacks = maxStacks;
        }

        public override void HandleImpact(Collider impactedObject, Vector3 hitPosition, Vector3 hitNormal, GunHandler gun)
        {
            base.HandleImpact(impactedObject, hitPosition, hitNormal, gun);

            for (int i = 0; i < Hits; i++)
            {
                if (_hitObjects[i].transform.root.TryGetComponent(out ISlowable slowable))
                {
                    slowable.Slow(SlowPercent, SlowTime, IsStackable, MaxStacks, _hitObjects[i].transform.root.GetComponent<PhotonView>());
                }
            }
        }
    }
}