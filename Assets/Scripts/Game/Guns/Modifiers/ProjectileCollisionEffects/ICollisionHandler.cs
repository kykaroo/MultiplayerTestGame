using Game.Guns.Handlers;
using UnityEngine;

namespace Game.Guns.Modifiers.ProjectileCollisionEffects
{
    public interface ICollisionHandler
    {
        void HandleImpact(Collider impactedObject, Vector3 hitPosition, Vector3 hitNormal, GunHandler gun);
    }
}