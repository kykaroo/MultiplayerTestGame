using System;
using UnityEngine;

namespace Game.Guns.Configs
{
    [CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Gun Trail Config", order = 4)]
    public class TrailConfigurationScriptableObject : ScriptableObject, ICloneable
    {
        public Material Material;
        public AnimationCurve WidthCurve;
        public float Duration = 0.5f;
        public float MinVertexDistance = 0.2f;
        public Gradient Color;

        public float MissDistance = 100f;
        public float SimulationSpeed = 100f;
        public object Clone()
        {
            TrailConfigurationScriptableObject config = CreateInstance<TrailConfigurationScriptableObject>();
            CloneCreator.CopyValues(this, config);
            return config;
        }
    }
}
