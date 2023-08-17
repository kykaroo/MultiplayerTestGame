using Game.Camera.GUIs;
using Game.Player;
using Network;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Camera.States
{
    public class DeathGuiState : GuiBaseStateWithPayload<PlayerController>
    {
        private DeathGui _deathGui;
        private PlayerController _playerController;
        private readonly PlayerManager _playerManager;

        public DeathGuiState(GuiFactory guiFactory, PlayerManager playerManager) : base(guiFactory)
        {
            _playerManager = playerManager;
        }

        protected override void OnEnter(PlayerController playerController)
        {
            _deathGui = GuiFactory.CreateGUI<DeathGui>();

            _playerController = playerController;
            
            _deathGui.OnRespawnButtonClick += Respawn;
            _deathGui.OnQuitButtonClick += QuitGame;
        }

        protected override void OnExit()
        {
            _deathGui.OnRespawnButtonClick -= Respawn;
            _deathGui.OnQuitButtonClick -= QuitGame;
            
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
            StateMachine.SetState<HudState, PlayerController>(_playerManager.CreateController());
        }
    }
}