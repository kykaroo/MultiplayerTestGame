using Game.Camera.GUIs;
using Game.Player;
using Object = UnityEngine.Object;

namespace Game.Camera.States
{
    public class InitializeState : GuiStateBase
    {
        private Initialize _initialize;
        private readonly PlayerManager _playerManager;

        public InitializeState(GuiFactory guiFactory, PlayerManager playerManager) : base(guiFactory)
        {
            _playerManager = playerManager;
        }
        
        protected override void OnEnter()
        {
            _initialize = GuiFactory.CreateGUI<Initialize>();

            _playerManager.OnRoomEntered += StateMachine.SetState<PreGameState>;
        }

        protected override void OnExit()
        {
            _playerManager.OnRoomEntered -= StateMachine.SetState<PreGameState>;
            
            Object.Destroy(_initialize);
        }
    }
}