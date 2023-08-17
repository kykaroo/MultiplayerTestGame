using System;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class RoomCamera : MonoBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private Button quitButton;

    public event Action OnJoinButtonClick;
    public event Action OnQuitButtonClick;

    private void Start()
    {
        joinButton.onClick.AddListener(OnJoinClick);
        quitButton.onClick.AddListener(OnQuitClick);
    }

    protected virtual void OnJoinClick()
    {
        OnJoinButtonClick?.Invoke();
    }

    protected virtual void OnQuitClick()
    {
        OnQuitButtonClick?.Invoke();
    }
}
