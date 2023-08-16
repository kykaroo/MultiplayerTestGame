namespace Lobby.States
{
    public abstract class MenuStateBase : IMenuState
    {
        protected MenuStateMachine StateMachine;
        protected readonly MenuFactory MenuFactory;

        protected MenuStateBase(MenuFactory menuFactory)
        {
            MenuFactory = menuFactory;
        }

        public void Enter(MenuStateMachine menuStateMachine)
        {
            StateMachine = menuStateMachine;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }
    }
}