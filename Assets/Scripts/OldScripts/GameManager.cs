using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    
    void Start()
    {

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab!=null)
            {
                int randomPoint = Random.Range(6, 3);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, randomPoint, 0f), Quaternion.identity);
            }
            else
            {
                Debug.Log("Place playerPrefab!");
            }
        }
    }
}
