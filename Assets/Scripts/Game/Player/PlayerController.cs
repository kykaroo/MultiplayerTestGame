using System;
using Game.ItemSystem;
using Game.ItemSystem.Weapon;
using Network;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.Player
{
    public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
    {
        [SerializeField] private float mouseSensitivityX;
        [SerializeField] private float mouseSensitivityY;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float smoothTime;

        [SerializeField] private Rigidbody playerBody;
        [SerializeField] private GameObject itemHolder;
        [SerializeField] private GameObject nickNameCanvas;

        [SerializeField] private Transform cameraHolder;
        [SerializeField] private UnityEngine.Camera playerCameraPrefab;
    
        [SerializeField] private UsableItem[] items;

        private int _itemIndex;
        private int _previousItemIndex = -1;

        public bool grounded;

        private float _verticalLookRotation;

        private Vector3 _smoothMoveVelocity;
        private Vector3 _moveAmount;
        private const float MaxHealth = 100f;
        private float _currentHealth = MaxHealth;
        private float _currentReloadTime;
        private UnityEngine.Camera _camera;
        private PlayerManager _playerManager;

        private PhotonView PV;

        public event Action<float, float> OnHealthChange;
        public event Action OnDeath;
        public event Action<UsableItem> OnItemChange;

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
                EquipItem(0);
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

            Look();
            Move();
            Jump();
            InputCheck();

            EquipItemCheck();
            FallOffMapCheck();
        }

        private void InputCheck()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                items[_itemIndex].M1ButtonAction();
            }

            if (Input.GetKeyDown(KeyCode.R))
            { 
                items[_itemIndex].RButtonAction();         
            }
        }

        private void EquipItemCheck()
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    EquipItem(i);
                    break;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (_itemIndex >= items.Length - 1)
                {
                    EquipItem(0);
                }
                else
                {
                    EquipItem(_itemIndex + 1);
                }
            }
            else
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    if (_itemIndex <= 0)
                    {
                        EquipItem(items.Length - 1);
                    }
                    else
                    {
                        EquipItem(_itemIndex - 1);
                    }
                }
            }
        }

        private void FallOffMapCheck()
        {
            if (transform.position.y < -10f)
            {
                Die();
            }
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                playerBody.AddForce(transform.up * jumpForce);
            }
        }

        private void Move()
        {
            Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            _moveAmount =
                Vector3.SmoothDamp(_moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref _smoothMoveVelocity, smoothTime);
        }

        void Look()
        {
            transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensitivityX));

            _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);

            cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
        }

        public void SetGroundedState(bool grounded)
        {
            this.grounded = grounded;
        }

        private void FixedUpdate()
        {
            if (!PV.IsMine)
                return;
        
            playerBody.MovePosition(playerBody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
        }

        void EquipItem(int index)
        {
            if (index == _previousItemIndex)
                return;

            _itemIndex = index;

            items[_itemIndex].itemGameObject.SetActive(true);

            if (_previousItemIndex != -1)
            {
                items[_previousItemIndex].itemGameObject.SetActive(false);
            }

            _previousItemIndex = _itemIndex;

            OnItemChange?.Invoke(items[_itemIndex]);

            items[_itemIndex].SetCamera(_camera);
            items[_itemIndex].OnItemChange();

            if (PV.IsMine)
            {
                var hash = new Hashtable();
                hash.Add("itemIndex", _itemIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && Equals(targetPlayer, PV.Owner))
            {
                EquipItem((int)changedProps["itemIndex"]);
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

        private void InstantiateItems()
        {
            foreach (var item in items)
            {
                PhotonNetwork.Instantiate("", gameObject.transform.position, quaternion.identity);
            }
        }
    }
}

