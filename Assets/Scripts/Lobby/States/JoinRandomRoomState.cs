using Photon.Pun;
using UnityEngine;

internal class JoinRandomRoomState : MenuStateBase
{
    private JoinRandomRoomMenu _joinRandomRoomMenu;

    public JoinRandomRoomState(MenuFactory menuFactory) : base(menuFactory)
    {
    }

    protected override void OnEnter()
    {
        CreateJoinRandomRoomMenu();
        PhotonNetwork.JoinRandomRoom();
        // Sample
        // PhotonNetwork.OnDisconnectFromRoom += OnDisconnect;
    }

    protected override void OnExit()
    {
        Object.Destroy(_joinRandomRoomMenu.gameObject);
        // Sample
        // PhotonNetwork.OnDisconnectFromRoom -= OnDisconnect;
    }

    private void CreateJoinRandomRoomMenu()
    {
        _joinRandomRoomMenu = MenuFactory.CreateMenuWindow<JoinRandomRoomMenu>();
        _joinRandomRoomMenu.OnClickBack += OnBackButtonClicked;
    }

    // Sample

    private void OnDisconnect()
    {
        Debug.LogError("Произошел дисконект");
        StateMachine.SetState<MainMenuState>();
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