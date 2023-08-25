using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SurfaceManager
{
    [CreateAssetMenu(menuName = "Impact System/Surface", fileName = "Surface")]
    public class Surface : ScriptableObject
    {
        [Serializable] 
        public class SurfaceImpactTypeEffect
        {
            public ImpactType ImpactType;
            public SurfaceEffect SurfaceEffect;
        }

        public List<SurfaceImpactTypeEffect> impactTypeEffects = new();
    }
}