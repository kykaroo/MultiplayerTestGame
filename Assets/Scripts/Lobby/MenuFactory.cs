using System;
using UnityEngine;

namespace Lobby
{
    public class MenuFactory : MonoBehaviour
    {
        [SerializeField] private Transform rootForUI;
    
        public T CreateMenuWindow<T>() where T : MonoBehaviour
        {
            T menuWindowPrefab = Resources.Load<T>($"Lobby/MenuPrefabs/{Convert.ToString(typeof(T).Name)}");
            T menuWindow = Instantiate(menuWindowPrefab, rootForUI);
            return menuWindow;
        }
    }
}
