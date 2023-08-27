using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace Game.Camera.GUIs
{
    public class DeathGui : MonoBehaviour
    {
        [SerializeField] public Button respawnButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button customizeButton;
        [SerializeField] public TextMeshProUGUI respawnText;

        public event Action OnQuitButtonClick;
        public event Action OnRespawnButtonClick;
        public event Action OnCustomizeButtonClick;

        protected virtual void OnRespawnClick()
        {
            OnRespawnButtonClick?.Invoke();
        }

        protected virtual void OnQuitClick()
        {
            OnQuitButtonClick?.Invoke();
        }

        protected virtual void OnCustomizeClick()
        {
            OnCustomizeButtonClick?.Invoke();
        }
    
        private void Start()
        {
            quitButton.onClick.AddListener(OnQuitClick);
            respawnButton.onClick.AddListener(OnRespawnClick);
            customizeButton.onClick.AddListener(OnCustomizeClick);
        }
    }
}
