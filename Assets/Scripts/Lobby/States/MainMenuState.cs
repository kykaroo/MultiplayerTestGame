using Photon.Pun;
using UnityEngine;

public class MainMenuState : MenuStateBase
{
    private MainMenu _mainMenu;

    public MainMenuState(MenuFactory menuFactory) : base(menuFactory)
    {
    }

    protected override void OnEnter()
    {
        _mainMenu = MenuFactory.CreateMenuWindow<MainMenu>();
        
        _mainMenu.OnClickCreateRoom += OnMainMenuCreateRoomButtonClicked;
        _mainMenu.OnClickExit += Application.Quit;
        _mainMenu.OnClickRoomList += OnShowRoomListButtonClicked;
        _mainMenu.OnClickRandomRoom += OnJoinRandomRoomButtonClicked;
        _mainMenu.OnClickJoinById += OnClickJoinById;
        _mainMenu.OnClickOptions += OnOptionsButtonClicked;
    }

    protected override void OnExit()
    {
        Object.Destroy(_mainMenu.gameObject);
        
        _mainMenu.OnClickCreateRoom -= OnMainMenuCreateRoomButtonClicked;
        _mainMenu.OnClickExit -= Application.Quit;
        _mainMenu.OnClickRoomList -= OnShowRoomListButtonClicked;
        _mainMenu.OnClickRandomRoom -= OnJoinRandomRoomButtonClicked;
        _mainMenu.OnClickJoinById -= OnClickJoinById;
        _mainMenu.OnClickOptions -= OnOptionsButtonClicked;
    }
    
    private void OnClickJoinById()
    {
        StateMachine.SetState<JoinByIDMenuState>();
    }

    private void OnMainMenuCreateRoomButtonClicked()
    {
        StateMachine.SetState<CreateRoomMenuState>();
    }

    private void OnOptionsButtonClicked()
    {
        StateMachine.SetState<OptionsMenuState>();
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        StateMachine.SetState<RoomListMenuState>();
    }

    private void OnJoinRandomRoomButtonClicked()
    {
        StateMachine.SetState<JoinRandomRoomState>();
    }
}