namespace Game.Camera.States
{
    public abstract class GuiBaseStateWithPayload<T> : IPayloadedState<T>
    {
        protected GuiStateMachine StateMachine;
        protected readonly GuiFactory GuiFactory;

        protected GuiBaseStateWithPayload(GuiFactory guiFactory)
        {
            GuiFactory = guiFactory;
        }

        public void Enter(GuiStateMachine guiStateMachine, T payload)
        {
            StateMachine = guiStateMachine;
            OnEnter(payload);
        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnExit() { }

        protected virtual void OnEnter(T player) { }
    }
}