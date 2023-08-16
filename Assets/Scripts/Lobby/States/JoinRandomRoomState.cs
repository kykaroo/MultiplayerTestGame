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
        _joinRandomRoomMenu = MenuFactory.CreateMenuWindow<JoinRandomRoomMenu>();
        
        _joinRandomRoomMenu.OnClickBack += OnBackButtonClicked;
        
        PhotonNetwork.JoinRandomRoom();
        // Sample
        // PhotonNetwork.OnDisconnectFromRoom += OnDisconnect;
    }

    protected override void OnExit()
    {
        Object.Destroy(_joinRandomRoomMenu.gameObject);
        
        _joinRandomRoomMenu.OnClickBack -= OnBackButtonClicked;
        
        // Sample
        // PhotonNetwork.OnDisconnectFromRoom -= OnDisconnect;
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