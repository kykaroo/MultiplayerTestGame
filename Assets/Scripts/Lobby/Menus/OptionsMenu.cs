using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby.Menus
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Button backButton;
    
        public event Action OnClickBack;

        protected virtual void Back()
        {
            OnClickBack?.Invoke();
        }

        private void Start()
        {
            backButton.onClick.AddListener(Back);
        }
    }
}
