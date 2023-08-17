namespace Game.Camera.States
{
    public abstract class GuiStateBase : IGuiState
    {
        protected GuiStateMachine StateMachine;
        protected readonly GuiFactory GuiFactory;

        protected GuiStateBase(GuiFactory guiFactory)
        {
            GuiFactory = guiFactory;
        }

        public void Enter(GuiStateMachine guiStateMachine)
        {
            StateMachine = guiStateMachine;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnExit() { }

        protected virtual void OnEnter() { }
    }
}
