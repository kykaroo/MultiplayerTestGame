using Lobby.Menus;
using Photon.Pun;
using UnityEngine;

namespace Lobby.States
{
    public class OptionsMenuState : MenuStateBase
    {
        private OptionsMenu _optionsMenu;

        public OptionsMenuState(MenuFactory menuFactory) : base(menuFactory)
        {
        }

        protected override void OnEnter()
        {
            _optionsMenu = MenuFactory.CreateMenuWindow<OptionsMenu>();
        
            _optionsMenu.OnClickBack += OnBackButtonClicked;
        }

        protected override void OnExit()
        {
            Object.Destroy(_optionsMenu.gameObject);
        
            _optionsMenu.OnClickBack -= OnBackButtonClicked;
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
}