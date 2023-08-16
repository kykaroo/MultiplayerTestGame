using System;
using System.Collections.Generic;
using Lobby.States;

namespace Lobby
{
    public class MenuStateMachine
    {
        private readonly Dictionary<Type, IMenuState> _states;
        private IMenuState _currentState;

        public MenuStateMachine(Dictionary<Type,IMenuState> states)
        {
            _states = states;
        }

        public void SetState<T>() where T : IMenuState
        {
            _currentState?.Exit();
            _currentState = _states[typeof(T)];
            _currentState.Enter(this);
        }
    }
}