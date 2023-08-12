using System.Collections.Generic;
using System.Linq;
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
                t.Open();
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
        foreach (var t in menus.Where(t => t.open))
        {
            CloseMenu(t);
        }
        menu.Open();
    }
}
