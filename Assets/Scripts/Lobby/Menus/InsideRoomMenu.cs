using System;
using Lobby.Items;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Menus
{
    public class InsideRoomMenu : MonoBehaviour
    {
        private PlayerListItem _playerListPrefab;
        [SerializeField] public Transform playerListContent;
        [SerializeField] public GameObject startGameButton;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Button startButton;
        [SerializeField] public TextMeshProUGUI roomInfoText;

        public event Action OnClickLeave;
        public event Action OnClickStartGame;

        private void Leave()
        {
            OnClickLeave?.Invoke();
        }

        private void StartGame()
        {
            OnClickStartGame?.Invoke();
        }

        private void Start()
        {
            leaveButton.onClick.AddListener(Leave);
            startButton.onClick.AddListener(StartGame);
        }

        private void Awake()
        {
            _playerListPrefab = Resources.Load<PlayerListItem>("ItemForListPrefabs/PlayerListItemPrefab");
        }

        public PlayerListItem CreatePlayerListItem()
        {
            PlayerListItem playerListGameObject = Instantiate(_playerListPrefab, playerListContent, true);
            playerListGameObject.transform.localScale = Vector3.one;
            playerListGameObject.transform.localPosition = playerListContent.position;
            return playerListGameObject;
        }

        public bool TryActivateStartButton()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                startGameButton.SetActive(true);
                return true;
            }

            startGameButton.SetActive(false);
            return false;
        }
    }
}
