using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;

namespace Game.Camera.GUIs
{
    public class PreGameGui : MonoBehaviour
    {
        [SerializeField] public Button spawnButton;
        [SerializeField] private Button quitButton;
        [SerializeField] public TextMeshProUGUI respawnText;

        public event Action OnSpawnButtonClick;
        public event Action OnQuitButtonClick;

        private void Start()
        {
            spawnButton.onClick.AddListener(OnSpawnClick);
            quitButton.onClick.AddListener(OnQuitClick);
        }

        protected virtual void OnSpawnClick()
        {
            OnSpawnButtonClick?.Invoke();
        }

        protected virtual void OnQuitClick()
        {
            OnQuitButtonClick?.Invoke();
        }
    }
}
