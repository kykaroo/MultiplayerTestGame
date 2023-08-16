using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomListMenuState : MenuStateBase
{
    private RoomListMenu _roomListMenu;

    private Dictionary<string, RoomInfo> _cachedRoomList;

    private Dictionary<string, RoomListItem> _roomListGameObjects;
    private readonly NetworkManager _networkManager;

    public RoomListMenuState(MenuFactory menuFactory, NetworkManager networkManager) : base(menuFactory)
    {
        _networkManager = networkManager;
    }

    protected override void OnEnter()
    {
        _roomListMenu = MenuFactory.CreateMenuWindow<RoomListMenu>();
        
        _networkManager.OnRoomListUpdateEvent += UpdatePlayersList;
        _roomListMenu.OnClickBackButton += OnBackButtonClicked;
    }

    protected override void OnExit()
    {
        Object.Destroy(_roomListMenu.gameObject);
        
        _networkManager.OnRoomListUpdateEvent -= UpdatePlayersList;
        _roomListMenu.OnClickBackButton -= OnBackButtonClicked;
    }

    private static void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
    
        PhotonNetwork.JoinRoom(roomName);
    }


    void ClearRoomListView()
    {
        foreach (var roomListGameObject in _roomListGameObjects.Values)
        {
            Object.Destroy(roomListGameObject.gameObject);
        }
        
        _roomListGameObjects.Clear();
    }


    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        StateMachine.SetState<MainMenuState>();
    }

    private void UpdatePlayersList(List<RoomInfo> roomList)
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
            RoomListItem roomListItem = _roomListMenu.CreateRoomListItem();
            roomListItem.SetUp(room);

            roomListItem.OnClickJoinRoom += () => OnJoinRoomButtonClicked(room.Name);

            _roomListGameObjects.Add(room.Name, roomListItem);
        }
    }
}