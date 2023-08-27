using Game.Camera.GUIs;
using Game.Player;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Camera.States
{
    using Player = Game.Player.Player;
    public class DeathGuiState : GuiBaseStateWithPayload<Player>
    {
        private DeathGui _deathGui;
        private Player _playerController;
        private readonly PlayerManager _playerManager;

        public DeathGuiState(GuiFactory guiFactory, PlayerManager playerManager) : base(guiFactory)
        {
            _playerManager = playerManager;
        }

        protected override void OnEnter(Player player)
        {
            _deathGui = GuiFactory.CreateGUI<DeathGui>();

            _playerController = player;

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
            PhotonNetwork.Destroy(_playerController.gameObject);
            StateMachine.SetState<HudState, Player>(_playerManager.CreatePlayer());
        }
        
        private void OpenCustomizeMenu()
        {
            PhotonNetwork.Destroy(_playerController.gameObject);
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