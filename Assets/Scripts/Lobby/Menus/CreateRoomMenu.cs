using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Menus
{
    public class CreateRoomMenu : MonoBehaviour
    {
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_InputField roomNameInputField; [SerializeField] private TMP_InputField maxPlayersInputField;

        public string RoomName => roomNameInputField.text;
        public string MaxPlayer => maxPlayersInputField.text;

        public event Action OnClickCreateRoom;
        public event Action OnClickBack;

        protected virtual void CreateRoom()
        {
            OnClickCreateRoom?.Invoke();
        }

        protected virtual void Back()
        {
            OnClickBack?.Invoke();
        }

        private void Start()
        {
            createRoomButton.onClick.AddListener(CreateRoom);
            backButton.onClick.AddListener(Back);
        }
    }
}
