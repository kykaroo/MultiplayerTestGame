using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Button backButton;
    
    [SerializeField] private Menu menu;
    
    public Menu Menu => menu;
    
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
