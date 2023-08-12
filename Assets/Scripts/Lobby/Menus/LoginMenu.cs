using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField loginInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Button loginButton;
    
    [SerializeField] private Menu menu;

    public string PlayerName => loginInputField.text;
    public string PasswordInputField => passwordInputField.text;

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
