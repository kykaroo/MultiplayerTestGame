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
        [Header("Base movement")]
        public float baseSpeed;
        [Tooltip("Умножается на скорость игрока для избежания огромных значений базовой скорости. Стандартно 10f.")]
        public float speedScale;
        public float jumpForce;
        public float sprintSpeedMultiplier;
        public float midAirMoveSpeedMultiplier;
        [Header("Anti-force")]
        public float groundDrag;
        public float airDrag;
        public float gravityScale;
        [Header("Jumping")]
        public float jumpCooldown;
        public bool autoJump;
        [Header("Sliding")] 
        public float minimumSpeedToSlide;
        public float slideInitialForce;
        public float timeToGetVelocityBoostFromSlideMove;
        public Animator playerBodyAnimator;
        [Header("Crouch")] 
        public float crouchSpeed;
        [Header("Items and camera")]
        public PlayerItemSelector itemSelector;
        [SerializeField] private Transform cameraHolderTransform;
        [SerializeField] private CameraHolder cameraHolder;
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
        private const float Gravity = -9.81f;
        private bool _isSliding;
        private float _actualSpeed;
        private Vector3 _moveDirection;
        private float _timerToGetVelocityBoostFromSlideMove;

        private Coroutine _slowCoroutine;

        public bool grounded;

        public event Action<float> OnReload;
        public event Action<int, int> OnAmmunitionUpdate;
        public event Action<string> OnSpeedUpdate;

        public MovementState state;
        private static readonly int IsSliding = Animator.StringToHash("IsSliding");

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            playerBody = GetComponent<Rigidbody>();
            playerBody.useGravity = false;
            
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

            if (!_isSliding)
            {
                playerBody.drag = grounded ? groundDrag : airDrag;
            }

            StateHandler();

            var velocity = playerBody.velocity;
            _actualSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            OnSpeedUpdate?.Invoke(Math.Round(_actualSpeed, 3).ToString());

            _timerToGetVelocityBoostFromSlideMove -= Time.deltaTime;
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
                    playerBody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange); 
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
            
            var gameObjectTransform = transform;
            _moveDirection = gameObjectTransform.forward * _verticalInput +
                             gameObjectTransform.right * _horizontalInput;
        }

        private void Look()
        {
            transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensitivityX));

            _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -80f, 80f);
            
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

            playerBody.AddForce(Vector3.up * (Gravity * gravityScale), ForceMode.Force);

            if (grounded && _onSlope && !_isSliding)
            {
                playerBody.AddForce(GetSlopeMoveDirection(_moveDirection).normalized * (_currentSpeed * speedScale), ForceMode.Acceleration);    
            }

            if (grounded && !_onSlope && !_isSliding)
            {
                playerBody.AddForce(_moveDirection.normalized * (_currentSpeed * speedScale), ForceMode.Acceleration);
            }
            
            if (!grounded)
            {
                playerBody.AddForce(_moveDirection.normalized * (_currentSpeed * speedScale * midAirMoveSpeedMultiplier), ForceMode.Acceleration);   
            }
            
            if (grounded && Input.GetKey(KeyCode.LeftControl) && _actualSpeed >= minimumSpeedToSlide && !_isSliding)
            {
                StartSlide(_moveDirection);
            }

            if ((!Input.GetKey(KeyCode.LeftControl) || _actualSpeed <= crouchSpeed) && _isSliding)
            {
                StopSlide();
            }
        }

        private void StopSlide()
        {
            _isSliding = false;
            playerBodyAnimator.SetBool(IsSliding, false);
        }

        private void StartSlide(Vector3 moveDir)
        {
            _isSliding = true;
            playerBody.drag = 1f;
            playerBodyAnimator.SetBool(IsSliding, true);
            if (_timerToGetVelocityBoostFromSlideMove <= 0)
            {
                playerBody.AddForce(moveDir.normalized * (_actualSpeed * slideInitialForce), ForceMode.VelocityChange);
                _timerToGetVelocityBoostFromSlideMove = timeToGetVelocityBoostFromSlideMove;
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
