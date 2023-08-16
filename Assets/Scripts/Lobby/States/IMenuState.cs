namespace Lobby.States
{
    public interface IMenuState
    {
        void Enter(MenuStateMachine menuStateMachine);
        void Exit();
    }
}