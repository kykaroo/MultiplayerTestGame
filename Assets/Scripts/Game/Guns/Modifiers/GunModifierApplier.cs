using Game.Guns.Modifiers.ProjectileCollisionEffects;
using Game.Player;
using Game.SurfaceManager;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Modifiers
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private ImpactType impactTypeOverride;
        [SerializeField] private PlayerItemSelector itemSelector;
        private void Start()
        {
            if(!photonView.IsMine) return;

            // new ImpactTypeModifier
            // {
            //     Amount = impactTypeOverride
            // }.Apply(itemSelector.ActiveGun);
            //
            // itemSelector.ActiveGun.BulletImpactEffects = new ICollisionHandler[]
            // {
            //     new FrostModifier(2f, new(new Keyframe(0, 1), new Keyframe(1, 0.25f)), 
            //         0, 0.5f, 1, false, 1)
            // };
            
            // DamageModifier damageModifier = new()
            // {
            //     Amount = 1.2f,
            //     AttributeName = "DamageConfig/damageCurve"
            // };
            // damageModifier.Apply(itemSelector.ActiveGun);

            // DamageModifier damageModifier = new()
            // {
            //     Amount = 1.5f,
            //     AttributeName = "DamageConfig/damageCurve"
            // };
            // damageModifier.Apply(itemSelector.ActiveGun);
            //
            // Vector3Modifier spreadModifier = new Vector3Modifier()
            // {
            //     Amount = Vector3.zero,
            //     AttributeName = "ShootConfig/Spread"
            // };
            // spreadModifier.Apply(itemSelector.ActiveGun);
        }
    }
}