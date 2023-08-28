using System;
using System.Collections;
using Game.Player.PlayerInterfaces;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour, ISlowable
    {
        [Header("Player")]
        public float mouseSensitivityX;
        public float mouseSensitivityY;
        [Header("Movement")]
        public float baseSpeed;
        public float sprintSpeedMultiplier;
        public float jumpForce;
        public float smoothTime;
        public LayerMask groundMask;
        public float groundDrag;
        public float airDrag;
        public float jumpCooldown;
        [FormerlySerializedAs("airMultiplier")] public float midAirMoveSpeedMultiplier;
        public bool autoJump;
        [Header("Items and camera")]
        public PlayerItemSelector itemSelector;
        [SerializeField] private Transform cameraHolderTransform;
        [SerializeField] private CameraHolder cameraHolder;
        [SerializeField] private UnityEngine.Camera playerCameraPrefab;
        [Header("Hands")]
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [Header("Reload")]
        [SerializeField] private bool autoReload = true;
        [SerializeField] private Animator handsAnimator;
        [SerializeField] private bool animationReload;
        [Header("Needed only if animationReload = false")]
        [SerializeField] private float reloadTime;


        private float _verticalLookRotation;
        private Vector3 _smoothMoveVelocity;
        private Vector3 _moveAmount;
        private float horizontalInput;
        private float verticalInput;
        public Rigidbody _playerBody;
        private PhotonView _photonView;
        private bool isReloading;
        private float walkSpeed;
        private float sprintSpeed;
        private int slowStacks;
        private float currentMovementMultiplier = 1;
        private float jumpCooldownTimer;

        private Coroutine _slowCoroutine;

        public bool grounded;

        public event Action<float> OnReload;
        public event Action<int, int> OnAmmunitionUpdate;
        public event Action<string> OnSpeedUpdate; 

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _playerBody = GetComponent<Rigidbody>();
            
            walkSpeed = baseSpeed;
            sprintSpeed = baseSpeed * sprintSpeedMultiplier;
            
            cameraHolder.OnReloadEnded += EndReload;

            foreach (AnimationClip clip in handsAnimator.runtimeAnimatorController.animationClips)
            {
                switch (clip.name)
                {
                    case "Reloading":
                        reloadTime = clip.length;
                        break;
                }
            }
        }

        private void AmmunitionUpdate(int currentAmmo, int maxAmmo)
        {
            OnAmmunitionUpdate?.Invoke(itemSelector.ActiveGun.AmmoConfig.currentClipAmmo, itemSelector.ActiveGun.AmmoConfig.currentAmmo);
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;

            
            if (itemSelector.ActiveGun != null)
            {
                itemSelector.ActiveGun.OnAmmunitionUpdate += AmmunitionUpdate; // TODO исправить костыль
                itemSelector.ActiveGun.Tick(Input.GetMouseButton(0));
            }

            if (ShouldManualReload() || ShouldAutoReload())
            {
                isReloading = true;
                OnReload?.Invoke(reloadTime);
                if (animationReload)
                {
                    cameraHolder.PlayAnimation();
                }
                else
                {
                    StartCoroutine(ReloadTimer());
                }
            }
            
            Look();
            Inputs();
            if (autoJump && jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
            Jump();
            if (grounded)
            {
                _playerBody.drag = groundDrag;
            }
            else
            {
                _playerBody.drag = airDrag;
            }

            var velocity = _playerBody.velocity;
            OnSpeedUpdate?.Invoke(Math.Round(new Vector3(velocity.x, 0f, velocity.z).magnitude, 3).ToString());
        }

        private void Jump()
        {
            if (autoJump)
            {
                if (Input.GetKey(KeyCode.Space) && grounded && jumpCooldownTimer <= 0)
                {
                    var velocity = _playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    _playerBody.velocity = velocity;
                    _playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
                    jumpCooldownTimer = jumpCooldown;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) && grounded)
                {
                    var velocity = _playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    _playerBody.velocity = velocity;
                    _playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
            }
        }

        private void Inputs()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        void Look()
        {
            transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensitivityX));

            _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -80f, 80f);

            leftHand.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
            rightHand.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
            cameraHolderTransform.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
        }
        
        public void SetGroundedState(bool grounded)
        {
            this.grounded = grounded;
        }
        
        
        private void FixedUpdate()
        {
            if (!_photonView.IsMine)
                return;
            
            var moveDir = transform.forward * verticalInput + transform.right * horizontalInput;

            if (grounded)
            {
                _playerBody.AddForce(moveDir.normalized * ((Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * 10f), ForceMode.Force);
            }
            else
            {
                _playerBody.AddForce(moveDir.normalized * ((Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * 10f * midAirMoveSpeedMultiplier), ForceMode.Force);   
            }
        }

        private bool ShouldAutoReload()
        {
            return !isReloading && autoReload &&  itemSelector.ActiveGun.AmmoConfig.currentClipAmmo == 0 && itemSelector.ActiveGun.CanReload();
        }

        private bool ShouldManualReload()
        {
            return !isReloading && Input.GetKeyDown(KeyCode.R) && itemSelector.ActiveGun.CanReload();
        }

        private void EndReload()
        {
            if (!_photonView.IsMine) return;
            
            itemSelector.ActiveGun.EndReload();
            isReloading = false;
            OnAmmunitionUpdate?.Invoke(itemSelector.ActiveGun.AmmoConfig.currentClipAmmo, itemSelector.ActiveGun.AmmoConfig.currentAmmo);
        }

        private IEnumerator ReloadTimer()
        {
            yield return new WaitForSeconds(reloadTime);
            EndReload();
        }
        
        public void Dead()
        {
            cameraHolder.Dead();
        }

        public void Slow(float movementMultiplier, float slowTime, bool isStackable, int maxStacks, PhotonView targetPhotonView)
        {
            _photonView.RPC(nameof(RPC_Slow), targetPhotonView.Owner, movementMultiplier, slowTime, isStackable);
        }

        private void CalculateSlowPercent(float movementMultiplier)
        {
            if (slowStacks == 1) currentMovementMultiplier =  movementMultiplier;
            else currentMovementMultiplier +=  movementMultiplier / slowStacks;
        }

        private IEnumerator SlowDown(float slowTime)
        {
            walkSpeed = baseSpeed * currentMovementMultiplier;
            sprintSpeed = walkSpeed * sprintSpeedMultiplier;
            yield return new WaitForSeconds(slowTime);

            slowStacks = 0;
            currentMovementMultiplier = 1;
            walkSpeed = baseSpeed;
            sprintSpeed = walkSpeed * sprintSpeedMultiplier;
        }

        [PunRPC]
        void RPC_Slow(float movementMultiplier, float slowTime, bool isStackable)
        {
            slowStacks++;

            if (isStackable) CalculateSlowPercent(movementMultiplier);
            else currentMovementMultiplier = movementMultiplier;
            
            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
            }

            _slowCoroutine = StartCoroutine(SlowDown(slowTime));
        }
    }
}
