using Game.Player;
using UnityEngine;

namespace Game.Guns.Modifiers
{
    public class GunModifierApplier : MonoBehaviour
    {
        [SerializeField] private PlayerItemSelector itemSelector;
        private void Start()
        {
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