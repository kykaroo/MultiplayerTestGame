using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class ExplodeModifier : AreaOfEffect
    {
        public ExplodeModifier(float areaRadius, AnimationCurve damageFallOff, int baseDamage) : base(areaRadius, damageFallOff, baseDamage) { }
    }
}