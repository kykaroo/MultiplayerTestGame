using UnityEngine;

namespace Game.Player.PlayerInterfaces
{
    public interface IDamageable
    {
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
    
        public delegate void TakeDamageEvent(float maxHealth, float currentHealth);
        
        public event TakeDamageEvent OnHealthChange;

        public delegate void DeathEvent(Vector3 position);
        
        public event DeathEvent OnDeath;

        public void TakeDamage(float damage, string bodyPartHit);
    }
}
