using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoomListMenu : MonoBehaviour
{
    [SerializeField] private Button backButton;

    [SerializeField] private GameObject roomListContent;
    
    [SerializeField] private Menu menu;
    
    private GameObject _roomListEntryPrefab;

    // public GameObject roomListContent => RoomListContent;
    // public GameObject RoomListEntryPrefab => _roomListEntryPrefab;
    public Menu Menu => menu;

    public event Action OnClickBackButton;

    protected virtual void Back()
    {
        OnClickBackButton?.Invoke();
    }

    private void Start()
    {
        backButton.onClick.AddListener(Back);
    }

    private void Awake()
    {
        _roomListEntryPrefab = Resources.Load<GameObject>("ItemForListPrefabs/RoomListItemPrefab");
    }

    public GameObject CreateRoomListItem()
    {
        GameObject roomListEntryGameObject = Instantiate(_roomListEntryPrefab, roomListContent.transform, true);
        roomListEntryGameObject.transform.localScale = Vector3.one;
        roomListEntryGameObject.transform.localPosition = roomListContent.transform.position;
        return roomListEntryGameObject;
    }
}
