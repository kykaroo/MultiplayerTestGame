using System;
using Photon.Pun;
using Object = UnityEngine.Object;

public class JoinByIDMenuState : MenuStateBase
{
    private JoinByIDMenu _joinByIDMenu;

    public JoinByIDMenuState(MenuFactory menuFactory) : base(menuFactory)
    {
    }

    protected override void OnEnter()
    {
        _joinByIDMenu = MenuFactory.CreateMenuWindow<JoinByIDMenu>();
        _joinByIDMenu.OnClickJoin += () => OnJoinRoomButtonClicked(_joinByIDMenu.RoomName);
        _joinByIDMenu.OnClickBack += OnBackButtonClicked;
    }

    protected override void OnExit()
    {
        Object.Destroy(_joinByIDMenu.gameObject);
        
        _joinByIDMenu.OnClickBack -= OnBackButtonClicked;
    }

    private void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
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