using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using Game.Player;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Network
{
    public class PlayerManager : MonoBehaviour
    {
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
            _controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrafabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] {PV.ViewID});
        }

        private void Start()
        {
            if (PV.IsMine)
            {
                CreateController();
            }
        }

        public void Die()
        {
            PhotonNetwork.Destroy(_controller);
            CreateController();
        
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
    }
}
