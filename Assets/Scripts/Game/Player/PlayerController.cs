using Game.ItemSystem;
using Game.ItemSystem.Weapon;
using Network;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.Player
{
    public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
    {
        [SerializeField] private float mouseSensitivityX, mouseSensitivityY, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    
        [SerializeField] private Rigidbody playerBody;

        [SerializeField] private GameObject cameraHolder;
    
        [SerializeField] private UsableItem[] items;

        [SerializeField] private GameObject healthBarGameObject;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private GameObject HUD;
        [SerializeField] private Image reloadIndicator;
        public TextMeshProUGUI ammunitionDisplay;

        [SerializeField] private int itemIndex;
        private int _previousItemIndex = -1;

        private PlayerManager PlayerManager;


        public bool grounded;

        private float _verticalLookRotation;

        private Vector3 _smoothMoveVelocity;
        private Vector3 _moveAmount;
        private const float MaxHealth = 100f;
        private float _currentHealth = MaxHealth;
        private float _currentReloadTime;

        private PhotonView PV;

        private void Awake()
        {
            playerBody = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();

            PlayerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                EquipItem(0);
            }
            else
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(playerBody);
                Destroy(HUD);
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

            if (reloadIndicator.fillAmount >= 1f && reloadIndicator.gameObject.activeSelf)
            {
                reloadIndicator.gameObject.SetActive(false);
            }
            else
            {
                reloadIndicator.fillAmount += 1f / _currentReloadTime * Time.deltaTime;
            }
        }

        private void InputCheck()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                items[itemIndex].M1ButtonAction();
            }

            if (Input.GetKeyDown(KeyCode.R))
            { 
                items[itemIndex].RButtonAction();         
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
                if (itemIndex >= items.Length - 1)
                {
                    EquipItem(0);
                }
                else
                {
                    EquipItem(itemIndex + 1);
                }
            }
            else
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    if (itemIndex <= 0)
                    {
                        EquipItem(items.Length - 1);
                    }
                    else
                    {
                        EquipItem(itemIndex - 1);
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
        
            playerBody.MovePosition(playerBody.position + transform.TransformDirection(_moveAmount)*Time.fixedDeltaTime);
        }

        void EquipItem(int index)
        {
            if (index == _previousItemIndex)
                return;

            itemIndex = index;

            items[itemIndex].itemGameObject.SetActive(true);

            if (_previousItemIndex != -1)
            {
                items[_previousItemIndex].GetComponent<BulletWeapon>().OnReload -= WeaponReload;
                items[_previousItemIndex].GetComponent<BulletWeapon>().OnAmmunitionChange -= UpdateAmmunitionDisplay;
                items[_previousItemIndex].itemGameObject.SetActive(false);
            }

            _previousItemIndex = itemIndex;
            
            items[itemIndex].GetComponent<BulletWeapon>().OnReload += WeaponReload;
            items[itemIndex].GetComponent<BulletWeapon>().OnAmmunitionChange += UpdateAmmunitionDisplay;
            items[itemIndex].OnItemChange();

            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("itemIndex", itemIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        private void UpdateAmmunitionDisplay(int magazineSize, int bulletsLeft, int bulletPerTap)
        {
            if (ammunitionDisplay != null)
            {
                ammunitionDisplay.text =
                    $"{bulletsLeft / bulletPerTap} / {magazineSize / bulletPerTap}";
            }
        }

        private void WeaponReload(float reloadTime)
        {
            _currentReloadTime = reloadTime;
            reloadIndicator.gameObject.SetActive(true);
            reloadIndicator.fillAmount = 0f;
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && Equals(targetPlayer, PV.Owner))
            {
                EquipItem((int)changedProps["itemIndex"]);
            }
        }

        public void TakeDamage(float damage)
        {
            PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage); 
        }

        [PunRPC]
        void RPC_TakeDamage(float damage, PhotonMessageInfo info)
        {
            _currentHealth -= damage;

            healthBarImage.fillAmount = _currentHealth / MaxHealth;
        
            healthBarGameObject.SetActive(MaxHealth - _currentHealth != 0);

            if (_currentHealth <= 0)
            {
                Die();
                PlayerManager.Find(info.Sender).GetKill();
            }
        }

        private void Die()
        {
            PlayerManager.Die();
        }
    }
}

