using System;
using System.Collections.Generic;
using Lobby.States;
using UnityEngine;

namespace Lobby
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private MenuFactory menuFactory;

        private MenuStateMachine _stateMachine;

        private void Start()
        {
            SetupStates();

            _stateMachine.SetState<LoginMenuState>();
        }

        private void SetupStates()
        {
            _stateMachine = new(new()
            {
                { typeof(LoginMenuState), new LoginMenuState(menuFactory, networkManager) },
                { typeof(MainMenuState), new MainMenuState(menuFactory) },
                { typeof(JoinRandomRoomState), new JoinRandomRoomState(menuFactory) },
                { typeof(CreateRoomMenuState), new CreateRoomMenuState(menuFactory) },
                { typeof(JoinByIDMenuState), new JoinByIDMenuState(menuFactory) },
                { typeof(InsideRoomMenuState), new InsideRoomMenuState(menuFactory, networkManager) },
                { typeof(RoomListMenuState), new RoomListMenuState(menuFactory, networkManager) },
                { typeof(OptionsMenuState), new OptionsMenuState(menuFactory) },
            });
            networkManager.OnJoinedRoomEvent += () => _stateMachine.SetState<InsideRoomMenuState>();
        }
    }
}