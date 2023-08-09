using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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

    public void SetUp(Player player)
    {
        playerName.text = player.NickName;
        if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerIndicator.SetActive(true); 
        }
        else
        {
            playerIndicator.SetActive(false); 
        }
    }
}
