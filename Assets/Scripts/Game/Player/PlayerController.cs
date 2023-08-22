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
        
        
        private float _currentReloadTime;

        private PhotonView PV;

        public event Action OnFallOffMap;


        private void Awake()
        {
            playerBody = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                Instantiate(playerCameraPrefab, cameraHolder);
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
                OnFallOffMap?.Invoke();
            }
        }

        [PunRPC]
        public void RPC_DisableHands()
        {
            itemHolder.SetActive(false);
            nickNameCanvas.SetActive(false);
        }
    }
}

