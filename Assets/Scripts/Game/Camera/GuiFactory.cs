using System;
using UnityEngine;

namespace Game.Camera
{
    public class GuiFactory : MonoBehaviour
    {
        [SerializeField] private Transform rootForGui;
    
        public T CreateGUI<T>() where T : MonoBehaviour
        {
            T guiPrefab = Resources.Load<T>($"Game/Cams/{Convert.ToString(typeof(T).Name)}");
            T gui = Instantiate(guiPrefab, rootForGui);
            return gui;
        }
    }
}