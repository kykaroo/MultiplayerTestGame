using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Lobby.Items
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject playerIndicator;
        [SerializeField] private TextMeshProUGUI playerName;

        private Player _player;
        // public GameObject PlayerIndicator => playerIndicator;
    
        /*public string PlayerName
    {
        set => playerName.text = value;
    }*/

        public void SetUp(Player player)
        {
            _player = player;
            playerName.text = player.NickName;
            playerIndicator.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
        }
    
        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (Equals(_player, otherPlayer))
            {
                Destroy(gameObject);
            }
        }
    }
}
