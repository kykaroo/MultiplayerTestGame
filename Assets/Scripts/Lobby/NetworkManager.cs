using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private Dictionary<string, RoomInfo> _cachedRoomList;
    private Dictionary<string, RoomListItem> _roomListGameObjects;

    [SerializeField] private MenuManager menuManager;

    private void Start()
    { 
        _cachedRoomList = new();  
        _roomListGameObjects = new();
    }

    public override void OnConnected()
    {
        print("Connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " is connected to photon");
        menuManager.OpenMenu(menuManager.MainMenu.Menu);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        menuManager.OpenMenu(menuManager.InsideRoomMenu.Menu);
        
        menuManager.InsideRoomMenu.TryActivateStartButton();

        menuManager.InsideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var playerListItem = menuManager.InsideRoomMenu.CreatePlayerListItem();
            playerListItem.SetUp(player);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                _cachedRoomList[room.Name] = room;
            }
        }

        foreach (RoomInfo room in _cachedRoomList.Values)
        {
            RoomListItem roomListItem = menuManager.RoomListMenu.CreateRoomListItem();
            roomListItem.SetUp(room);
            
            roomListItem.OnClickJoinRoom += () => MenuManager.OnJoinRoomButtonClicked(room.Name);

            _roomListGameObjects.Add(room.Name, roomListItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        menuManager.InsideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        var playerListItem = menuManager.InsideRoomMenu.CreatePlayerListItem();
        playerListItem.SetUp(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        menuManager.InsideRoomMenu.roomInfoText.text =
            $"Room name: {PhotonNetwork.CurrentRoom.Name} \n\r  Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
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
    
    public override void OnLeftRoom()
    {
        menuManager.OpenMenu(menuManager.MainMenu.Menu);
    }

    public override void OnMasterClientSwitched(Player newMaserClient)
    {
        menuManager.InsideRoomMenu.startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short s, string room)
    {
        print($"Room \"{room}\" does not exist");
    }
    
    void ClearRoomListView()
    {
        foreach (var roomListGameObject in _roomListGameObjects.Values)
        {
            Destroy(roomListGameObject.gameObject);
        }
        
        _roomListGameObjects.Clear();
    }
}
