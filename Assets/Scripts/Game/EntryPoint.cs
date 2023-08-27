using System.Collections;
using System.Collections.Generic;
using Game.Camera;
using Game.Camera.States;
using Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GuiFactory guiFactory; 
        [SerializeField] private UnityEngine.Camera lobbyCamera;
        [SerializeField] private string playerManagerPrefabPath;
        private PlayerManager _playerManager;
        
        private GuiStateMachine _stateMachine;
        private void Start()
        {
            StartCoroutine(Create());
        }
        

        private void SetupStates()
        {
            _stateMachine = new(new()
            {
                { typeof(DeathGuiState), new DeathGuiState(guiFactory, _playerManager) },
                { typeof(HudState), new HudState(guiFactory) },
                { typeof(PreGameState), new PreGameState(guiFactory, _playerManager, lobbyCamera) },
                { typeof(InitializeState), new InitializeState(guiFactory, _playerManager) },
                { typeof(CustomizeGuiState), new CustomizeGuiState(guiFactory, lobbyCamera) }
            });
        }
        
        private IEnumerator Create()
        {
            yield return new WaitForSeconds(0.1f);
            _playerManager = PhotonNetwork.Instantiate(playerManagerPrefabPath, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            SetupStates();
            _stateMachine.SetState<PreGameState>();
        }
    }
}