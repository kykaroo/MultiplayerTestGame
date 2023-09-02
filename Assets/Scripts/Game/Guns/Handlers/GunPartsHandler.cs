using UnityEngine;

namespace Game.Guns.Handlers
{
    public class GunPartsHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        public ParticleSystem ParticleSystem => particleSystem;
    }
}