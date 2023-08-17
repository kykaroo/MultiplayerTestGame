namespace Game.Camera.States
{
    public interface IGuiState : IExitableState
    {
        void Enter(GuiStateMachine guiStateMachine);
    }
    
    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(GuiStateMachine guiStateMachine, TPayload payload);
    }
    
    public interface IExitableState
    {
        void Exit();
    }
}
