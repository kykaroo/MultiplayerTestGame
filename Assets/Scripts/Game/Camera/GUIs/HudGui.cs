using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Camera.GUIs
{
    public class HudGui : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarGameObject;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private Image reloadIndicator;
        [SerializeField] private TextMeshProUGUI ammunitionDisplay;
        [SerializeField] private TextMeshProUGUI speedDisplay;

        public float currentReloadTime;
        
        public GameObject HealthBarGameObject => healthBarGameObject;

        public Image HealthBarImage => healthBarImage;

        public Image ReloadIndicator => reloadIndicator;

        public TextMeshProUGUI AmmunitionDisplay => ammunitionDisplay;
        
        public TextMeshProUGUI SpeedDisplay => speedDisplay;


        private void Update()
        {
            if (reloadIndicator.fillAmount >= 1f && reloadIndicator.gameObject.activeSelf)
            {
                reloadIndicator.gameObject.SetActive(false);
            }
            else
            {
                reloadIndicator.fillAmount += 1f / currentReloadTime * Time.deltaTime;
            }
        }
    }
}
