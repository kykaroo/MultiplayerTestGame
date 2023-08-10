using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UserNameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView playerPv;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = playerPv.Owner.NickName;
    }
}
