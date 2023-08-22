﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.ItemSystem.NewSystem
{
    [CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config")]
    public class DamageConfigScriptableObject : ScriptableObject
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
    }
}