using System.Collections;
using Game.Camera;
using Game.Camera.States;
using Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Game
{
    public class EntryPoint : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GuiFactory guiFactory; 
        [SerializeField] private UnityEngine.Camera lobbyCamera;
        [SerializeField] private string playerManagerPrefabPath;
        
        private PlayerManager _playerManager;
        private Game.Player.Player _player;
        
        private GuiStateMachine _stateMachine;
        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            StartCoroutine(Create());
        }
        

        private void SetupStates()
        {
            _stateMachine = new(new()
            {
                { typeof(DeathGuiState), new DeathGuiState(guiFactory, _playerManager, _player) },
                { typeof(HudState), new HudState(guiFactory, _player) },
                { typeof(PreGameState), new PreGameState(guiFactory, _playerManager, lobbyCamera, _player) },
                { typeof(InitializeState), new InitializeState(guiFactory, _playerManager) },
                { typeof(CustomizeGuiState), new CustomizeGuiState(guiFactory, lobbyCamera) }
            });
        }
        
        private IEnumerator Create()
        {
            yield return new WaitForSeconds(0.1f);
            _playerManager = PhotonNetwork.Instantiate(playerManagerPrefabPath, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
            _player = _playerManager.CreatePlayer();
            _player.lobbyCamera = lobbyCamera.gameObject;
            SetupStates();
            _stateMachine.SetState<PreGameState>();
        }
    }
}