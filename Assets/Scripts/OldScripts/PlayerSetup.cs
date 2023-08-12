using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] playerChildGameObjects;

    public GameObject playerUIPrefab;
    private PlayerMovementController playerMovementController;

    public Camera PlayerCamera;
    

    private Shooting shooter;

    
    void Start()
    {
        shooter = GetComponent<Shooting>();
        playerMovementController = GetComponent<PlayerMovementController>();

        if (photonView.IsMine)
        {
            foreach (GameObject gameObject in playerChildGameObjects)
            {
                gameObject.SetActive(false);
            }
            
            //Instantiate PlayerUI
            GameObject playerUIGameObject = Instantiate(playerUIPrefab);
            playerMovementController.joystick = playerUIGameObject.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();

            playerUIGameObject.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(shooter.Fire);

            PlayerCamera.enabled = true;
        }
        else
        {
            foreach (GameObject gameObject in playerChildGameObjects)
            {
                gameObject.SetActive(true);
            }
            
            playerMovementController.enabled = false;

            PlayerCamera.enabled = false;
        }
    }
}
