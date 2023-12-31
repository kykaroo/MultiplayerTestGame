using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Menus
{
    public class JoinByIDMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomNameInputField;

        [SerializeField] private Button joinButton;
        [SerializeField] private Button backButton;
    
        public string RoomName => roomNameInputField.text;

        public event Action OnClickJoin;
        public event Action OnClickBack;

        protected virtual void Join()
        {
            OnClickJoin?.Invoke();
        }

        protected virtual void Back()
        {
            OnClickBack?.Invoke();
        }

        private void Start()
        {
            joinButton.onClick.AddListener(Join);
            backButton.onClick.AddListener(Back);
        }
    }
}
