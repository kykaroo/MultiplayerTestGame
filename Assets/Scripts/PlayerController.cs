using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public GameObject playerGameObject;
    public string playerName;
    public float moveSpeed = 1;
    public int health;
    private Vector2 moveDelta;
    public FixedJoystick _joystick;
    public BoxCollider2D BoxCollider2D;
    public Sprite playerSprite;
    public Button _fireButton;
    public float fireCooldown;
    public float fireCooldownTimer;
    public GameObject bullet;
    public TextMeshPro playerNameText;

    private void Start()
    {
        ButtonInitialize();
    }
    
    private void FixedUpdate()
    {
        Move();
        
        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
        else
        {
            fireCooldownTimer = 0;
        }
    }

    private void Move()
    {
        moveDelta = new Vector2(_joystick.Horizontal * moveSpeed, _joystick.Vertical * moveSpeed);
        if (moveDelta is { x: 0, y: 0 }) return;
        
        transform.Translate(moveDelta * Time.deltaTime);
    }

    private void ButtonInitialize()
    {
        _fireButton.onClick.AddListener(Fire);
    }

    private void Fire()
    {
        if (fireCooldownTimer == 0)
        {
            print("Shoot");
            fireCooldownTimer = fireCooldown;
            GameObject.Instantiate(bullet);
        }
        else
        {
            print($"On cooldown: {fireCooldownTimer} left");
        }
    }
}

