using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoomListMenu : MonoBehaviour
{
    [SerializeField] private Button backButton;

    [SerializeField] private Transform roomListContent;
    
    [SerializeField] private Menu menu;
    
    private RoomListItem _roomListEntryPrefab;

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
        _roomListEntryPrefab = Resources.Load<RoomListItem>("ItemForListPrefabs/RoomListItemPrefab");
    }

    public RoomListItem CreateRoomListItem()
    {
        RoomListItem roomListEntryGameObject = Instantiate(_roomListEntryPrefab, roomListContent, true);
        roomListEntryGameObject.transform.localScale = Vector3.one;
        roomListEntryGameObject.transform.localPosition = roomListContent.position;
        return roomListEntryGameObject;
    }
}
