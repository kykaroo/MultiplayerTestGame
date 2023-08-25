using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config")]
    public class DamageConfigScriptableObject : ScriptableObject, ICloneable
    {
        public ParticleSystem.MinMaxCurve damageCurve;

        private void Reset()
        {
            damageCurve.mode = ParticleSystemCurveMode.Curve;
        }

        public float GetDamage(float distance = 0)
        {
            return Mathf.CeilToInt(damageCurve.Evaluate(distance, Random.value));
        }

        public object Clone()
        {
            DamageConfigScriptableObject config = CreateInstance<DamageConfigScriptableObject>();
            config.damageCurve = damageCurve;
            return config;
        }
    }
}