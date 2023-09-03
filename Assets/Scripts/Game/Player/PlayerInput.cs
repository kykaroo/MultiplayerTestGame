using System;
using System.Collections;
using Game.Player.Movement;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Player")]
        public float mouseSensitivityX;
        public float mouseSensitivityY;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private BaseMovement baseMovement;
        [SerializeField] private JumpHandler jumpHandler;
        [SerializeField] private ReloadHandler reloadHandler;
        [SerializeField] private SlideMovement slideMovement;
        [SerializeField] private CrouchHandler crouchHandler;
        [SerializeField] private CameraHolder cameraHolder;
        [SerializeField] private PlayerSurfaceCheck playerSurfaceCheck;
        [Header("Items and camera")]
        [SerializeField] private PlayerItemSelector itemSelector;
        [SerializeField] private PlayerItemSelector playerItemSelector;
        [SerializeField] private Transform cameraHolderTransform;


        private float _verticalLookRotation;
        private float _horizontalInput;
        private float _verticalInput;
        public Rigidbody playerBody;
        private float _actualSpeed;
        private Vector3 _moveDirection;
        
        public event Action<string> OnSpeedUpdate;

        public MovementState state;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            playerBody = GetComponent<Rigidbody>();
            playerBody.useGravity = false;
        }

        public enum MovementState
        {
            Walking,
            Sprinting,
            MidAir,
            Crouching
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;
            
            itemSelector.ActiveGun.Tick(Input.GetMouseButton(0));

            Look();
            _moveDirection = Inputs();
            baseMovement.TryToMove(_moveDirection);
            jumpHandler.TryToJump();
            reloadHandler.TryToReload();
            
            var velocity = playerBody.velocity;
            _actualSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            OnSpeedUpdate?.Invoke(Math.Round(_actualSpeed, 3).ToString());

            SlideCheck();
            StateHandler();
            GunChangeCheck();
        }

        private void GunChangeCheck()
        {
            for (var i = 0; i < playerItemSelector.AllPlayerGuns.Count; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    playerItemSelector.EquipItem(i);
                    break;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (playerItemSelector.currentListIndex + 1 > playerItemSelector.AllPlayerGuns.Count - 1)
                {
                    playerItemSelector.EquipItem(0);
                }
                else
                {
                    playerItemSelector.EquipItem(playerItemSelector.currentListIndex + 1);
                }
            }
            else
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    if (playerItemSelector.currentListIndex - 1 < 0)
                    {
                        playerItemSelector.EquipItem(playerItemSelector.AllPlayerGuns.Count - 1);
                    }
                    else
                    {
                        playerItemSelector.EquipItem(playerItemSelector.currentListIndex - 1);
                    }
                }
            }
        }

        private void SlideCheck()
        {
            slideMovement.UpdateActualSpeed(_actualSpeed);
            slideMovement.TryToSlide();
            slideMovement.TryToStopSlide(baseMovement.baseSpeed, baseMovement.crouchSpeedMultiplier);
        }

        private void StateHandler()
        {
            switch (playerSurfaceCheck.Grounded)
            {
                case true when Input.GetKey(KeyCode.LeftControl):
                    state = MovementState.Crouching;
                    crouchHandler.SetCrouchState(true);
                    baseMovement.SetSpeed(MovementState.Crouching);
                    return;
                case true when Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl):
                    state = MovementState.Sprinting;
                    crouchHandler.SetCrouchState(false);
                    baseMovement.SetSpeed(MovementState.Sprinting);
                    return;
                case true:
                    state = MovementState.Walking;
                    crouchHandler.SetCrouchState(false);
                    baseMovement.SetSpeed(MovementState.Walking);
                    return;
                case false when Input.GetKey(KeyCode.LeftControl):
                    state = MovementState.Crouching;
                    crouchHandler.SetCrouchState(true);
                    baseMovement.SetSpeed(MovementState.MidAir);
                    return;
                case false:
                    state = MovementState.MidAir;
                    crouchHandler.SetCrouchState(false);
                    baseMovement.SetSpeed(MovementState.MidAir);
                    return;
            }
        }

        private Vector3 Inputs()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            var gameObjectTransform = transform;
            return gameObjectTransform.forward * _verticalInput +
                             gameObjectTransform.right * _horizontalInput;
        }

        private void Look()
        {
            transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensitivityX));

            _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -80f, 80f);
            
            cameraHolderTransform.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
        }

        public void Dead()
        {
            cameraHolder.Dead();
        }

        public void Respawn()
        {
            cameraHolder.Respawn();
            _verticalLookRotation = 0;
        }
    }
}
