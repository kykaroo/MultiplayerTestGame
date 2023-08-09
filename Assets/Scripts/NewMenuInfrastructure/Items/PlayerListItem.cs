using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private GameObject playerIndicator;
    [SerializeField] private TextMeshProUGUI playerName;
    public GameObject PlayerIndicator => playerIndicator;
    
    public string PlayerName
    {
        set => playerName.text = value;
    }
}
