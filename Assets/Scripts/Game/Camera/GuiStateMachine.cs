using System;
using System.Collections.Generic;
using Game.Camera.States;
using Game.Player;
using UnityEngine;

namespace Game.Camera
{
    public class GuiStateMachine : MonoBehaviour
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _currentState;

        public GuiStateMachine(Dictionary<Type, IExitableState> states)
        {
            _states = states;
        }

        public void SetState<T>() where T : class, IGuiState
        {
            _currentState?.Exit();
            var state = GetState<T>();
            state.Enter(this);
            _currentState = state;
        }
        
        public void SetState<T, TPayload>(TPayload payload) where T : class, IPayloadedState<TPayload>
        {
            _currentState?.Exit();
            var state = GetState<T>();
            state.Enter(this, payload);
            _currentState = state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }
}
