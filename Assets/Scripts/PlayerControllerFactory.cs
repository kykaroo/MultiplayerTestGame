using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class PlayerControllerFactory
{
    /*private GameObject playerGameObject;
    private GameObject bullet;
    public PlayerControllerFactory()
    {
        playerGameObject = Resources.Load<GameObject>("Player"); 
        bullet = bullet = Resources.Load<GameObject>("Bullet");
    }
   public PlayerController CreatePlayer(string playerName, int health, float moveSpeed, Sprite characterSprite,
       float fireCooldown, FixedJoystick fixedJoystick, Button fireButton)
   { 
       Object.Instantiate(playerGameObject);
       playerGameObject.AddComponent<PlayerController>();
       PlayerController playerController = playerGameObject.GetComponent<PlayerController>(); 
       playerController.playerGameObject = playerGameObject; 
       playerController.playerName = playerName; 
       playerController.health = health; 
       playerController.moveSpeed = moveSpeed; 
       playerController.BoxCollider2D = playerGameObject.GetComponent<BoxCollider2D>(); 
       playerController.playerSprite = characterSprite; 
       playerController.fireCooldown = fireCooldown; 
       playerController.bullet = bullet; 
       playerController.playerNameText = playerGameObject.GetComponent<TextMeshPro>(); 
       playerController.playerNameText.text = playerName;
       //TODO Понять почему не присваивается fixedJoystick и fireButton
       playerController._joystick = fixedJoystick;
       playerController._fireButton = fireButton;
       return playerController;
   }*/
}

