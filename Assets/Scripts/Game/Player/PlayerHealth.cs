using System;
using Network;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private float _maxHealth = 100f;
        private float _currentHealth;
        private PhotonView _photonView;
        
        public float CurrentHealth 
        {
            get => _currentHealth;
            private set => _currentHealth = value;
        }

        public float MaxHealth
        {
            get => _maxHealth;
            private set => _maxHealth = value;
        }

        public event IDamageable.TakeDamageEvent OnHealthChange;


        public event IDamageable.DeathEvent OnDeath;

        public void TakeDamage(float damage, string bodyPartHitName)
        {
            float damageTaken = Mathf.Clamp(damage, 0, _currentHealth);
            
            switch (bodyPartHitName)
            {
                case "Head":
                    damageTaken *= 1.5f;
                    break;
                case "Body":
                    break;
                case "Legs":
                    damageTaken *= 0.5f;
                    break;
                default:
                    break;
            }
            
            //Данные обновляет только игрок, получивший урон
            _photonView.RPC(nameof(RPC_TakeDamage), _photonView.Owner, damageTaken);
        }

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _photonView = GetComponent<PhotonView>();
        }

        [PunRPC]
        void RPC_TakeDamage(float damage, PhotonMessageInfo info)
        {
            _currentHealth -= damage;

            if (damage != 0)
            {
                OnHealthChange?.Invoke(_maxHealth, _currentHealth);
            }

            if (_currentHealth <= 0 && damage != 0)
            {
                OnDeath?.Invoke(transform.position);
                _currentHealth = 0;
                PlayerManager.Find(info.Sender).GetKill();
            }
        }
    }
}