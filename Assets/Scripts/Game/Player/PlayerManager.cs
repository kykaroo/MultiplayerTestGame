using System;
using System.Linq;
using ExitGames.Client.Photon;
using Game.Camera.GUIs;
using Game.Spawn;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PreGameGui _preGameGUI;
        private DeathGui _deathGUI;
        public float respawnTimer;
        public float respawnTime;
        private float _defaultRespawnTime = 5f;

        public PhotonView _photonView;

        private int _kills;
        private int _deaths;
        
        public event Action OnRoomEntered;
        public event Action OnRespawnUnavailable;
        public event Action OnRespawnAvailable;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        internal Player CreatePlayer()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            return PhotonNetwork.Instantiate("Game/PhotonPrefabs/Player", spawnPoint.position, spawnPoint.rotation, 0,
                new object[] { _photonView.ViewID }).GetComponent<Player>();
        }

        private void Start()
        {
            if (_photonView.IsMine)
            {
                OnRoomEntered?.Invoke();
            }
        }

        public void AddDeathToCounter()
        {
            _deaths++;
            respawnTimer = _defaultRespawnTime;

            var hash = new Hashtable { { "deaths", _deaths } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public void GetKill()
        {
            _photonView.RPC(nameof(RPC_GetKill), _photonView.Owner);
        }

        [PunRPC]
        void RPC_GetKill()
        {
            _kills++;

            Hashtable hash = new Hashtable { { "kills", _kills } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public static PlayerManager Find(Photon.Realtime.Player player)
        {
            return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => Equals(x._photonView.Owner, player));
        }
        
        private void Update()
        {
            if (respawnTimer <= 0)
            {
                OnRespawnAvailable?.Invoke();
            }
            else
            {
                respawnTimer -= Time.deltaTime;
                respawnTime = (float)Math.Round(respawnTimer, 1);
                OnRespawnUnavailable?.Invoke();
            }
        }
    }
}
