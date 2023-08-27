using Game.Camera.GUIs;
using Game.Player;
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

            _preGameGui.OnSpawnButtonClick += Spawn;
            _preGameGui.OnQuitButtonClick += QuitGame;
            _playerManager.OnRespawnAvailable += EnableButton;
            _playerManager.OnRespawnUnavailable += UpdateTimer;
            _camera.gameObject.SetActive(true);
        }

        protected override void OnExit()
        {
            _preGameGui.OnSpawnButtonClick -= Spawn;
            _preGameGui.OnQuitButtonClick -= QuitGame;
            _playerManager.OnRespawnAvailable -= EnableButton;
            _playerManager.OnRespawnUnavailable -= UpdateTimer;
            _camera.gameObject.SetActive(false);
            
            Object.Destroy(_preGameGui.gameObject);
        }
        
        private void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
        
        private void Spawn()
        {
            StateMachine.SetState<HudState, Player>(_playerManager.CreatePlayer());
        }
        
        private void EnableButton()
        {
            _preGameGui.respawnText.text = "Respawn";
            _preGameGui.spawnButton.interactable = true;
        }
        
        private void UpdateTimer()
        {
            _preGameGui.spawnButton.interactable = false;
            _preGameGui.respawnText.text = $"Respawn: {_playerManager.respawnTime}";
        }
    }
}