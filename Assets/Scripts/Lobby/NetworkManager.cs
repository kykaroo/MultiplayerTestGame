using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public event Action OnJoinedRoomEvent;
    public event Action OnLeftRoomEvent;
    public event Action<List<RoomInfo>> OnRoomListUpdateEvent;
    public event Action<Player> OnPlayerEnteredRoomEvent;
    public event Action<Player> OnPlayerLeftRoomEvent;
    public event Action<Player> OnMasterClientSwitchedEvent;

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnected() => print("Connected to internet");

    public override void OnCreatedRoom() => print(PhotonNetwork.CurrentRoom.Name + " is created");

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);

        var roomName = $"Room {Random.Range(1000, 10000)}";

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 20
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnJoinedRoom() => OnJoinedRoomEvent?.Invoke();

    public override void OnJoinRoomFailed(short s, string room) => print($"Room \"{room}\" does not exist");

    public override void OnLeftRoom() => OnLeftRoomEvent?.Invoke();

    public override void OnMasterClientSwitched(Player newMaserClient) => OnMasterClientSwitchedEvent?.Invoke(newMaserClient);

    public override void OnPlayerEnteredRoom(Player newPlayer) => OnPlayerEnteredRoomEvent?.Invoke(newPlayer);

    public override void OnPlayerLeftRoom(Player otherPlayer) => OnPlayerLeftRoomEvent?.Invoke(otherPlayer);

    public override void OnRoomListUpdate(List<RoomInfo> roomList) => OnRoomListUpdateEvent?.Invoke(roomList);
}