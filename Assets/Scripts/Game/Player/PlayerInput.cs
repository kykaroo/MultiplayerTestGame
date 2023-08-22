using Photon.Pun;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public float mouseSensitivityX;
        public float mouseSensitivityY;
        public float sprintSpeed;
        public float walkSpeed;
        public float jumpForce;
        public float smoothTime;
        [SerializeField] private PlayerItemSelector itemSelector;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private UnityEngine.Camera playerCameraPrefab;
        private float _verticalLookRotation;
        
        private Vector3 _smoothMoveVelocity;
        private Vector3 _moveAmount;
        public Rigidbody _playerBody;
        private PhotonView PV;
        
        public bool grounded;

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
            _playerBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!PV.IsMine) return;

            if (itemSelector.ActiveGun != null)
            {
                itemSelector.ActiveGun.Tick(Input.GetMouseButton(0));
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
        
            _playerBody.MovePosition(_playerBody.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
        }
    }
}
