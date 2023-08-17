using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Items
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI roomPlayers;
        [SerializeField] private Button joinRoomButton;
    
        public event Action OnClickJoinRoom;
        
        protected virtual void JoinRoom()
        {
            OnClickJoinRoom?.Invoke();
        }
    
        private void Start()
        {
            joinRoomButton.onClick.AddListener(JoinRoom);
        }

        public void SetUp(RoomInfo roomInfo)
        {
            roomName.text = roomInfo.Name;
            roomPlayers.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        }
    }
}
