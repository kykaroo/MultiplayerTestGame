using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config")]
    public class DamageConfigScriptableObject : ScriptableObject
    {
        [SerializeField] private ParticleSystem.MinMaxCurve damageCurve;

        public ParticleSystem.MinMaxCurve DamageCurve => damageCurve;
    }
}