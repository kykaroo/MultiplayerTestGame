using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private List<Menu> menus = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddMenu(Menu menu)
    {
        menus.Add(menu);
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu t in menus)
        {
            if (t.menuName == menuName)
            {
                OpenMenu(t);
            }
            else if (t.open)
            {
                CloseMenu(t);
            }
        }
    }

    private void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void OpenMenu(Menu menu)
    {
        print(menus.Count + " menuCount");
        foreach (Menu t in menus)
        {
            print($"");
            if (t.open)
            {
                print($"{t} is closed");
                CloseMenu(t);
            }
        }
        
        print($"{menu} is opened");
        menu.Open();
    }
}
