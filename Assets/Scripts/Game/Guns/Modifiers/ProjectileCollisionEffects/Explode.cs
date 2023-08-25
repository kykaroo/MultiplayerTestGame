using Game.Guns.Configs;
using Game.Player;
using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class Explode : AreaOfEffect
    {
        public Explode(float areaRadius, AnimationCurve damageFallOff, int baseDamage) : base(areaRadius, damageFallOff, baseDamage) { }
    }
}