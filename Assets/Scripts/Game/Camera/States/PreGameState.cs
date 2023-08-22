using Game.Camera.GUIs;
using Game.Player;
using Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Camera.States
{
    using Player = Game.Player.Player;
    public class PreGameState : GuiStateBase
    {
        private PreGameGui _preGameGui;
        private readonly PlayerManager _playerManager;
        private UnityEngine.Camera _camera;

        public PreGameState(GuiFactory guiFactory, PlayerManager playerManager, UnityEngine.Camera camera) : base(guiFactory)
        {
            _playerManager = playerManager;
            _camera = camera;
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
            
            Object.Destroy(_camera.gameObject);
            Object.Destroy(_preGameGui.gameObject);
        }
        
        private void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
        
        private void JoinGame()
        {
            StateMachine.SetState<HudState, Player>(_playerManager.CreatePlayer());
        }
    }
}