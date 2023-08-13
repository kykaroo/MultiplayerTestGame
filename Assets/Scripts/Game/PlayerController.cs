using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    [SerializeField] private float mouseSensitivityX, mouseSensitivityY, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    
    [SerializeField] private Rigidbody playerBody;

    [SerializeField] private GameObject cameraHolder;
    
    [SerializeField] private Item[] items;

    [SerializeField] private GameObject healthBarGameObject;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private GameObject ui;

    [SerializeField] private int itemIndex;
    private int _previousItemIndex = -1;

    private PlayerManager PlayerManager;


    public bool grounded;

    private float _verticalLookRotation;

    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;
    private const float MaxHealth = 100f;
    private float _currentHealth = MaxHealth;

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
            Destroy(ui);
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

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

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
            items[_previousItemIndex].itemGameObject.SetActive(false);
        }

        _previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
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

