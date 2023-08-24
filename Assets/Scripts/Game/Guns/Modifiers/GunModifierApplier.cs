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
        }
    }
}