using System;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using Game.Player;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class PlayerManager : MonoBehaviour
    {
        private RoomCamera _roomCamera;
        private DeathCamera _deathCamera;

        private PhotonView PV;

        private GameObject _controller;

        private int _kills;
        private int _deaths;

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
        }

        void CreateController()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            _controller = PhotonNetwork.Instantiate("Game/PhotonPrefabs/PlayerController", spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                CreateRoomCamera();
            }
        }

        public void Die(GameObject cameraHolder)
        {
            PhotonNetwork.Destroy(_controller);
            CreateDeathCam(cameraHolder);

            _deaths++;

            var hash = new Hashtable { { "deaths", _deaths } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void GetKill()
        {
            PV.RPC(nameof(RPC_GetKill), PV.Owner);
        }

        [PunRPC]
        void RPC_GetKill()
        {
            _kills++;

            Hashtable hash = new Hashtable { { "kills", _kills } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public static PlayerManager Find(Player player)
        {
            return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => Equals(x.PV.Owner, player));
        }

        private void CreateRoomCamera()
        {
            _roomCamera = Instantiate(Resources.Load<GameObject>("Game/Cams/RoomCamera"), RoomManager.Instance.LobbyCameraAnchor.position,
                RoomManager.Instance.LobbyCameraAnchor.rotation).GetComponent<RoomCamera>();
            
            _roomCamera.OnJoinButtonClick += JoinGame;
            _roomCamera.OnQuitButtonClick += QuitGame;
        }
        
        private void CreateDeathCam(GameObject cameraHolder)
        {
            _deathCamera = Instantiate(Resources.Load<GameObject>("Game/Cams/DeathCam"), cameraHolder.transform.position,
                Quaternion.identity).GetComponent<DeathCamera>();

            _deathCamera.OnRespawnButtonClick += Respawn;
            _deathCamera.OnQuitButtonClick += QuitGame;
        }

        private void Respawn()
        {
            _deathCamera.OnRespawnButtonClick -= Respawn;
            _deathCamera.OnQuitButtonClick -= QuitGame;
            
            Destroy(_deathCamera.gameObject);
            CreateController();
        }

        private void JoinGame()
        {
            _roomCamera.OnJoinButtonClick -= JoinGame;
            _roomCamera.OnJoinButtonClick -= QuitGame;
            
            Destroy(_roomCamera.gameObject);
            CreateController();
        }

        private void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
    }
}
