﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CreateRoomMenuState : MenuStateBase
{
    private CreateRoomMenu _createRoomMenu;

    public CreateRoomMenuState(MenuFactory menuFactory) : base(menuFactory)
    {
    }

    protected override void OnEnter()
    {
        CreateCreateRoomMenu();
    }

    protected override void OnExit()
    {
        Object.Destroy(_createRoomMenu.gameObject);
    }

    private void CreateCreateRoomMenu()
    {
        _createRoomMenu = MenuFactory.CreateMenuWindow<CreateRoomMenu>();
        _createRoomMenu.OnClickCreateRoom += OnRoomCreateButtonClicked;
        _createRoomMenu.OnClickBack += OnBackButtonClicked;
    }

    public void OnRoomCreateButtonClicked()
    {
        var roomName = _createRoomMenu.RoomName;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 9999);
        }

        var roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)int.Parse(_createRoomMenu.MaxPlayer)
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        StateMachine.SetState<MainMenuState>();
    }
}