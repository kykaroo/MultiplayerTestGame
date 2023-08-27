using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Camera.GUIs
{
    public class CustomizeGui : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        private float _respawnTimer;

        public event Action OnBackButtonClick;

        protected virtual void OnBackClick()
        {
            OnBackButtonClick?.Invoke();
        }
        
        private void Start()
        {
            backButton.onClick.AddListener(OnBackClick);
        }
    }
}