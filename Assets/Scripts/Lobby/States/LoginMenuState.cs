using Photon.Pun;
using UnityEngine;


public class LoginMenuState : MenuStateBase
{
    private LoginMenu _loginMenu;

    public LoginMenuState(MenuFactory menuFactory) : base(menuFactory)
    {
    }

    protected override void OnEnter()
    {
        CreateLoginMenu();
        // TODO: загрузить логин пароль если есть и попробовать войти
    }

    protected override void OnExit()
    {
        Object.Destroy(_loginMenu.gameObject);
    }

    private void CreateLoginMenu()
    {
        _loginMenu = MenuFactory.CreateMenuWindow<LoginMenu>();
        _loginMenu.OnClickLogin += OnLoginButtonClicked;
    }

    private void OnLoginButtonClicked()
    {
        var playerName = _loginMenu.PlayerName;
        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Player name is invalid!");
        }

        if (PhotonNetwork.IsConnected)
        {
            StateMachine.SetState<MainMenuState>();
        }
        else
        {
            // TODO: StateMachine.SetState<ErrorLoginState>();
        }
    }
}