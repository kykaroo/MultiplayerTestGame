using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI roomPlayers;
    [SerializeField] private Button joinRoomButton;
    
    public event Action OnClickJoinRoom;
    
    public string RoomName
    {
        set => roomName.text = value;
    }
    
    public string RoomPlayers
    {
        set => roomPlayers.text = value;
    }
    
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
