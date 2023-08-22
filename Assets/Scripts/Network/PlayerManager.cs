using System;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using Game.Camera.GUIs;
using Game.Camera.States;
using Game.Player;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player = Game.Player.Player;

namespace Network
{
    public class PlayerManager : MonoBehaviour
    {
        private PreGameGui _preGameGUI;
        private DeathGui _deathGUI;

        private PhotonView PV;

        private int _kills;
        private int _deaths;
        
        public event Action OnRoomEntered;

        private void Awake()
        {
            PV = GetComponent<PhotonView>();
        }

        internal Player CreatePlayer()
        {
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            return PhotonNetwork.Instantiate("Game/PhotonPrefabs/Player", spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID}).GetComponent<Player>();
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                OnRoomEntered?.Invoke();
            }
        }

        public void AddDeathToCounter()
        {
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

        public static PlayerManager Find(Photon.Realtime.Player player)
        {
            return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => Equals(x.PV.Owner, player));
        }
    }
}
