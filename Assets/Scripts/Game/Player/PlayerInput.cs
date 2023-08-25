using System;
using System.Collections;
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
        public float sprintSpeed;
        public float walkSpeed;
        public float jumpForce;
        public float smoothTime;
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
        public Rigidbody _playerBody;
        private PhotonView _photonView;
        private bool isReloading = false;
        
        public bool grounded;

        public event Action<float> OnReload;
        public event Action<int, int> OnAmmunitionUpdate;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _playerBody = GetComponent<Rigidbody>();
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
            Move();
            Jump();
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                _playerBody.AddForce(transform.up * jumpForce);
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
        
            _playerBody.MovePosition(_playerBody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
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
    }
}
