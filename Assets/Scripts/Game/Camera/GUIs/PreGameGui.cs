using System;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Game.Camera.GUIs
{
    public class PreGameGui : MonoBehaviour
    {
        [SerializeField] private Button joinButton;
        [SerializeField] private Button quitButton;

        public event Action OnJoinButtonClick;
        public event Action OnQuitButtonClick;

        private void Start()
        {
            joinButton.onClick.AddListener(OnJoinClick);
            quitButton.onClick.AddListener(OnQuitClick);
        }

        protected virtual void OnJoinClick()
        {
            OnJoinButtonClick?.Invoke();
        }

        protected virtual void OnQuitClick()
        {
            OnQuitButtonClick?.Invoke();
        }
    }
}
