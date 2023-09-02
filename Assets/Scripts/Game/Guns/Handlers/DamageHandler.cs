using Game.Guns.Configs;
using UnityEngine;

namespace Game.Guns.Handlers
{
    public class DamageHandler
    {
        public ParticleSystem.MinMaxCurve DamageCurve;
        public DamageHandler(DamageConfigScriptableObject damageConfig)
        {
            DamageCurve = damageConfig.DamageCurve;
        }
        
        private void Reset()
        {
            DamageCurve.mode = ParticleSystemCurveMode.Curve;
        }

        public float GetDamage(float distance = 0)
        {
            return Mathf.CeilToInt(DamageCurve.Evaluate(distance, Random.value));
        }
    }
}