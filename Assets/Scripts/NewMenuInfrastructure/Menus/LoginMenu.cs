using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField LoginInputField;

    [SerializeField] Button loginButton;
    
    [SerializeField] private Menu menu;

    public string playerName => LoginInputField.text;

    public Menu Menu => menu;


    public event Action OnClickLogin;

    private void Login()
    {
        OnClickLogin?.Invoke();
    }

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
    }
}
