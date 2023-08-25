﻿using Game.Guns.ProjectileCollision;
using Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Game.Guns.Modifiers
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private PlayerItemSelector itemSelector;
        private void Start()
        {
            if(!photonView.IsMine) return;
            
            DamageModifier damageModifier = new()
            {
                Amount = 1.5f,
                AttributeName = "DamageConfig/damageCurve"
            };
            damageModifier.Apply(itemSelector.ActiveGun);

            Vector3Modifier spreadModifier = new Vector3Modifier()
            {
                Amount = Vector3.zero,
                AttributeName = "ShootConfig/Spread"
            };
            spreadModifier.Apply(itemSelector.ActiveGun);

            itemSelector.ActiveGun.bulletImpactEffects = new ICollisionHandler[]
            {
                new Explode(2f, new(new Keyframe(0, 1), new Keyframe(1, 0.25f)), 10)
            };
        }
    }
}