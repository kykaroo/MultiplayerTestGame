using Game.Player;
using Game.Player.PlayerInterfaces;
using UnityEngine;

namespace Game.Guns
{
    [RequireComponent(typeof(IDamageable))]
    public class SpawnParticleSystemOnDeath : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _deathSystem;
        public IDamageable Damageable;

        private void Awake()
        {
            Damageable = GetComponent<IDamageable>();
        }

        private void OnEnable()
        {
            Damageable.OnDeath += Damageable_OnDeath;
        }

        private void Damageable_OnDeath(Vector3 position)
        {
            Instantiate(_deathSystem, position, Quaternion.identity);
        }
    }
}