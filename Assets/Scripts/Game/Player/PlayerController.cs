using System;
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

        private PhotonView _photonView;

        public event Action OnFallOffMap;


        private void Awake()
        {
            playerBody = GetComponent<Rigidbody>();
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (_photonView.IsMine)
            {
                nickNameCanvas.SetActive(false);
                Instantiate(playerCameraPrefab, cameraHolder);
            }
            else
            {
                Destroy(playerBody);
            }
        }

        private void Update()
        {
            if (!_photonView.IsMine) return;

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
        private void RPC_DisableHands()
        {
            itemHolder.SetActive(false);
            nickNameCanvas.SetActive(false);
        }
        
        [PunRPC]
        private void RPC_EnableHands()
        {
            itemHolder.SetActive(true);
            nickNameCanvas.SetActive(true);
        }

        public void DisableHands()
        {
            _photonView.RPC(nameof(RPC_DisableHands), RpcTarget.All);
        }
        
        public void EnableHands()
        {
            photonView.RPC(nameof(RPC_EnableHands), RpcTarget.All);
            if (!_photonView.IsMine) return;
            nickNameCanvas.SetActive(false);
            cameraHolder.rotation = Quaternion.identity;
        }
    }
}

