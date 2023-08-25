﻿using System;
using Game.Player.PlayerInterfaces;
using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        public float maxHealth = 100f;
        public bool isDead;
        private float _currentHealth;
        private PhotonView _photonView;
        
        private void Awake()
        {
            _currentHealth = maxHealth;
            _photonView = GetComponent<PhotonView>();
        }
        
        public float CurrentHealth 
        {
            get => _currentHealth;
            private set => _currentHealth = value;
        }

        public float MaxHealth
        {
            get => maxHealth;
            private set => maxHealth = value;
        }

        public event IDamageable.TakeDamageEvent OnHealthChange;


        public event IDamageable.DeathEvent OnDeath;

        public void TakeDamage(float damage, string bodyPartHitName)
        {
            if (isDead) return;
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
            }
            
            _photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damageTaken);
        }

        [PunRPC]
        void RPC_TakeDamage(float damage, PhotonMessageInfo info)
        {
            if (isDead) return;
            
            _currentHealth -= damage;

            if (damage != 0)
            {
                OnHealthChange?.Invoke(maxHealth, _currentHealth);
            }

            if (_currentHealth <= 0 && damage != 0)
            {
                OnDeath?.Invoke(transform.position);
                _currentHealth = 0;
                if (_photonView.IsMine) GiveKill(info);
            }
        }

        void GiveKill(PhotonMessageInfo info)
        {
            PlayerManager killer = PlayerManager.Find(info.Sender);
            if (killer == null) throw new NullReferenceException("Убийца игрока не существует");
            if (killer.PV.IsMine) return;
            killer.GetKill();
        }
    }
}