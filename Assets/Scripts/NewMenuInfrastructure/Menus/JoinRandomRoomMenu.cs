using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRandomRoomMenu : MonoBehaviour
{
    [SerializeField] private Button BackButton;
    
    [SerializeField] private Menu menu;
    
    public Menu Menu => menu;
    

    public event Action OnClickBack;

    protected virtual void Back()
    {
        OnClickBack?.Invoke();
    }

    private void Start()
    {
        BackButton.onClick.AddListener(Back);
    }
}
