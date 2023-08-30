using Game.Camera.GUIs;
using Game.Player;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Camera.States
{
    using Player = Game.Player.Player;
    public class DeathGuiState : GuiStateBase
    {
        private DeathGui _deathGui;
        private Player _player;
        private readonly PlayerManager _playerManager;

        public DeathGuiState(GuiFactory guiFactory, PlayerManager playerManager, Player player) : base(guiFactory)
        {
            _playerManager = playerManager;
            _player = player;
        }

        protected override void OnEnter()
        {
            _deathGui = GuiFactory.CreateGUI<DeathGui>();

            _deathGui.OnCustomizeButtonClick += OpenCustomizeMenu;
            _deathGui.OnRespawnButtonClick += Respawn;
            _deathGui.OnQuitButtonClick += QuitGame;
            _playerManager.OnRespawnAvailable += EnableButton;
            _playerManager.OnRespawnUnavailable += UpdateTimer;
        }

        protected override void OnExit()
        {
            _deathGui.OnCustomizeButtonClick -= OpenCustomizeMenu;
            _deathGui.OnRespawnButtonClick -= Respawn;
            _deathGui.OnQuitButtonClick -= QuitGame;
            _playerManager.OnRespawnAvailable -= EnableButton;
            _playerManager.OnRespawnUnavailable -= UpdateTimer;
            
            Object.Destroy(_deathGui.gameObject);
        }

        private static void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }
        
        private void Respawn()
        {
            _player.Respawn();
            StateMachine.SetState<HudState>();
        }
        
        private void OpenCustomizeMenu()
        {
            StateMachine.SetState<CustomizeGuiState>();
        }
        
        private void EnableButton()
        {
            _deathGui.respawnText.text = "Respawn";
            _deathGui.respawnButton.interactable = true;
        }
        
        private void UpdateTimer()
        {
            _deathGui.respawnButton.interactable = false;
            _deathGui.respawnText.text = $"Respawn: {_playerManager.respawnTime}";
        }
    }
}