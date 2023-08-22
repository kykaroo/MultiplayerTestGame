using Game.Camera.GUIs;
using Game.Player;
using Network;
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
            StateMachine.SetState<HudState, Player>(_playerManager.CreatePlayer());
        }
    }
}