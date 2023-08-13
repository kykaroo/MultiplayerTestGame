using System;
using UnityEngine;

public class MenuFactory : MonoBehaviour
{
    [SerializeField] private Transform rootForUI;
    
    public T CreateMenuWindow<T>() where T : MonoBehaviour
    {
        T menuWindowPrefab = Resources.Load<T>($"MenuPrefabs/{Convert.ToString(typeof(T))}");
        T menuWindow = Instantiate(menuWindowPrefab, rootForUI);
        return menuWindow;
    }
}
