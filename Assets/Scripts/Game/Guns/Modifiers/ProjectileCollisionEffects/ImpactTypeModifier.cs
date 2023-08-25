using Game.Guns.Configs;
using Game.SurfaceManager;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
    {
        public override void Apply(GunScriptableObject gun)
        {
            gun.ImpactType = Amount;
        }
    }
}