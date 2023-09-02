using Game.Guns.Handlers;
using Game.SurfaceManager;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
    {
        public override void Apply(GunHandler gun)
        {
            gun.impactType = Amount;
        }
    }
}