using System;
using Game.ItemSystem.NewSystem;
using Network;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.Player
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private GameObject itemHolder;
        [SerializeField] private GameObject nickNameCanvas;

        [SerializeField] private Transform cameraHolder;
        [SerializeField] private UnityEngine.Camera playerCameraPrefab;

        private int _itemIndex;
        private int _previousItemIndex = -1;
        
        
        private const float MaxHealth = 100f;
        private float _currentHealth = MaxHealth;
        private float _currentReloadTime;
        private UnityEngine.Camera _camera;
        private PlayerManager _playerManager;

        private PhotonView PV;

        public event Action<float, float> OnHealthChange;
        public event Action OnDeath;

        private void Awake()
        {
            playerBody = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();

            _playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                _camera = Instantiate(playerCameraPrefab, cameraHolder);
            }
            else
            {
                Destroy(playerBody);
                return;
            }
        
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (!PV.IsMine) return;

            FallOffMapCheck();
        }
        
        private void FallOffMapCheck()
        {
            if (transform.position.y < -10f)
            {
                Die();
            }
        }

        public void TakeDamage(float damage, string bodyPartHitName)
        {
            PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage, bodyPartHitName); 
        }

        [PunRPC]
        void RPC_TakeDamage(float damage, string bodyPartHitName, PhotonMessageInfo info)
        {
            switch (bodyPartHitName)
            {
                case "Head":
                    damage *= 1.5f;
                    break;
                case "Body":
                    break;
                case "Legs":
                    damage *= 0.5f;
                    break;
            }
            _currentHealth -= damage;
            OnHealthChange?.Invoke(_currentHealth, MaxHealth);

            if (_currentHealth <= 0)
            {
                Die();
                PlayerManager.Find(info.Sender).GetKill();
            }
        }

        private void Die()
        {
            playerBody.constraints = RigidbodyConstraints.None;
            OnDeath?.Invoke();
            _playerManager.AddDeathToCounter();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PV.RPC(nameof(RPC_DisableHands), RpcTarget.All);
            
            enabled = false;
        }

        [PunRPC]
        void RPC_DisableHands()
        {
            itemHolder.SetActive(false);
            nickNameCanvas.SetActive(false);
        }
    }
}

