using System;
using System.Collections;
using Game.Player.PlayerInterfaces;
using Photon.Pun;
using UnityEngine;

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
        public float groundDrag;
        public float airDrag;
        public float jumpCooldown;
        public float midAirMoveSpeedMultiplier;
        public bool autoJump;
        [Header("Items and camera")]
        public PlayerItemSelector itemSelector;
        [SerializeField] private Transform cameraHolderTransform;
        [SerializeField] private CameraHolder cameraHolder;
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
        private float _horizontalInput;
        private float _verticalInput;
        public Rigidbody playerBody;
        private PhotonView _photonView;
        private bool _isReloading;
        private float _currentSpeed;
        private int _slowStacks;
        private float _currentMovementMultiplier = 1;
        private float _jumpCooldownTimer;
        private float _modifiedBaseSpeed;
        private RaycastHit _slopeHit;
        private bool _onSlope;

        private Coroutine _slowCoroutine;

        public bool grounded;

        public event Action<float> OnReload;
        public event Action<int, int> OnAmmunitionUpdate;
        public event Action<string> OnSpeedUpdate;

        public MovementState state;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            playerBody = GetComponent<Rigidbody>();
            
            _modifiedBaseSpeed = baseSpeed;
            
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
        
        public enum MovementState
        {
            Walking,
            Sprinting,
            MidAir,
            MidAirSprinting
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
                _isReloading = true;
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
            if (autoJump && _jumpCooldownTimer > 0)
            {
                _jumpCooldownTimer -= Time.deltaTime;
            }
            Jump();
            
            playerBody.drag = grounded ? groundDrag : airDrag;

            StateHandler();

            var velocity = playerBody.velocity;
            OnSpeedUpdate?.Invoke(Math.Round(new Vector3(velocity.x, 0f, velocity.z).magnitude, 3).ToString());
        }

        private void StateHandler()
        {
            switch (grounded)
            {
                case true when Input.GetKey(KeyCode.LeftShift):
                    state = MovementState.Sprinting;
                    _currentSpeed = _modifiedBaseSpeed * sprintSpeedMultiplier;
                    return;
                case true:
                    state = MovementState.Walking;
                    _currentSpeed = _modifiedBaseSpeed;
                    return;
                case false when Input.GetKey(KeyCode.LeftShift):
                    state = MovementState.MidAirSprinting;
                    _currentSpeed = _modifiedBaseSpeed * midAirMoveSpeedMultiplier * sprintSpeedMultiplier;
                    return;
                case false:
                    state = MovementState.MidAir;
                    _currentSpeed = _modifiedBaseSpeed * midAirMoveSpeedMultiplier;
                    return;
            }
        }

        private void Jump()
        {
            if (autoJump)
            {
                if (Input.GetKey(KeyCode.Space) && grounded && _jumpCooldownTimer <= 0)
                {
                    var velocity = playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    playerBody.velocity = velocity;
                    playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
                    _jumpCooldownTimer = jumpCooldown;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) && grounded)
                {
                    var velocity = playerBody.velocity;
                    velocity = new(velocity.x, 0f, velocity.z);
                    playerBody.velocity = velocity;
                    playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
            }
        }

        private void Inputs()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
        }

        private void Look()
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

            var gameObjectTransform = transform;
            var moveDir = gameObjectTransform.forward * _verticalInput + gameObjectTransform.right * _horizontalInput;

            if (grounded && _onSlope)
            {
                playerBody.AddForce(GetSlopeMoveDirection(moveDir).normalized * (_currentSpeed * 10f), ForceMode.Force);
            }

            if (grounded && !_onSlope)
            {
                playerBody.AddForce(moveDir.normalized * (_currentSpeed * 10f), ForceMode.Force);
            }
            
            if (!grounded)
            {
                playerBody.AddForce(moveDir.normalized * (_currentSpeed * 10f * midAirMoveSpeedMultiplier), ForceMode.Force);   
            }
        }

        public void SetOnSlopeState(bool onSlope, RaycastHit slopeHit)
        {
            _onSlope = onSlope;
            _slopeHit = slopeHit;
        }

        private bool ShouldAutoReload()
        {
            return !_isReloading && autoReload &&  itemSelector.ActiveGun.AmmoConfig.currentClipAmmo == 0 && itemSelector.ActiveGun.CanReload();
        }

        private bool ShouldManualReload()
        {
            return !_isReloading && Input.GetKeyDown(KeyCode.R) && itemSelector.ActiveGun.CanReload();
        }

        private void EndReload()
        {
            if (!_photonView.IsMine) return;
            
            itemSelector.ActiveGun.EndReload();
            _isReloading = false;
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
            if (_slowStacks == 1) _currentMovementMultiplier =  movementMultiplier;
            else _currentMovementMultiplier +=  movementMultiplier / _slowStacks;
        }

        private IEnumerator SlowDown(float slowTime)
        {
            _modifiedBaseSpeed = baseSpeed * _currentMovementMultiplier;
            yield return new WaitForSeconds(slowTime);

            _slowStacks = 0;
            _currentMovementMultiplier = 1;
            _modifiedBaseSpeed = baseSpeed;
        }

        [PunRPC]
        void RPC_Slow(float movementMultiplier, float slowTime, bool isStackable)
        {
            _slowStacks++;

            if (isStackable) CalculateSlowPercent(movementMultiplier);
            else _currentMovementMultiplier = movementMultiplier;
            
            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
            }

            _slowCoroutine = StartCoroutine(SlowDown(slowTime));
        }
        
        private Vector3 GetSlopeMoveDirection(Vector3 moveDir)
        {
            return Vector3.ProjectOnPlane(moveDir, _slopeHit.normal);
        }
    }
}
