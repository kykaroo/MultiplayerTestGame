using System;
using System.Collections.Generic;
using Game.Camera;
using Game.Camera.GUIs;
using Game.Camera.States;
using Game.Player;
using Network;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GuiFactory guiFactory; 
        [SerializeField] private UnityEngine.Camera preGameCamera; 
        private PlayerManager _playerManager;
        
        private GuiStateMachine _stateMachine;
        private void Start()
        {
            _playerManager = PhotonNetwork.Instantiate("Game/PhotonPrefabs/PlayerManager", Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            SetupStates();
            _stateMachine.SetState<PreGameState>();
        }
        

        private void SetupStates()
        {
            _stateMachine = new(new()
            {
                { typeof(DeathGuiState), new DeathGuiState(guiFactory, _playerManager) },
                { typeof(HudState), new HudState(guiFactory) },
                { typeof(PreGameState), new PreGameState(guiFactory, _playerManager, preGameCamera) },
                { typeof(InitializeState), new InitializeState(guiFactory, _playerManager) }
            });
        }
    }
}