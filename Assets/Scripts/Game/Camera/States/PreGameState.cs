using Game.Camera.GUIs;
using Game.Player;
using Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Camera.States
{
    public class PreGameState : GuiStateBase
    {
        private PreGameGui _preGameGui;
        private readonly PlayerManager _playerManager;

        public PreGameState(GuiFactory guiFactory, PlayerManager playerManager) : base(guiFactory)
        {
            _playerManager = playerManager;
        }
        
        protected override void OnEnter()
        {
            _preGameGui = GuiFactory.CreateGUI<PreGameGui>();

            _preGameGui.OnJoinButtonClick += JoinGame;
            _preGameGui.OnQuitButtonClick += QuitGame;
        }

        protected override void OnExit()
        {
            _preGameGui.OnJoinButtonClick -= JoinGame;
            _preGameGui.OnQuitButtonClick -= QuitGame;
            
            Object.Destroy(_preGameGui.gameObject);
        }
        
        private void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
        
        private void JoinGame()
        {
            StateMachine.SetState<HudState, PlayerController>(_playerManager.CreateController());
        }
    }
}