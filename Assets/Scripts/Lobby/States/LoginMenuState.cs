using Lobby.Menus;
using Photon.Pun;
using UnityEngine;

namespace Lobby.States
{
    public class LoginMenuState : MenuStateBase
    {
        private LoginMenu _loginMenu;
        private readonly NetworkManager _networkManager;

        public LoginMenuState(MenuFactory menuFactory, NetworkManager networkManager) : base(menuFactory)
        {
            _networkManager = networkManager;
        }

        protected override void OnEnter()
        {
            _loginMenu = MenuFactory.CreateMenuWindow<LoginMenu>();
        
            _loginMenu.OnClickLogin += OnLoginButtonClicked;
            _networkManager.OnConnectedToMasterServer += OnConnectedToMaster;
            // TODO: загрузить логин пароль если есть и попробовать войти
        }

        protected override void OnExit()
        {
            Object.Destroy(_loginMenu.gameObject);
        
            _loginMenu.OnClickLogin -= OnLoginButtonClicked;
            _networkManager.OnConnectedToMasterServer -= OnConnectedToMaster;
        }

        private void OnLoginButtonClicked()
        {
            var playerName = _loginMenu.PlayerName;
            if (!string.IsNullOrEmpty(playerName))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                _loginMenu.ConnectingText.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Player name is invalid!");
            }
        }

        private void OnConnectedToMaster()
        {
            StateMachine.SetState<MainMenuState>(); 
            // TODO: StateMachine.SetState<ErrorLoginState>();
        }
    }
}