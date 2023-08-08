using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public FixedJoystick joystick;

    private BoxCollider2D playerCollider;

    private Vector3 moveDelta;

    public float moveSpeed = 3f;

    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }


    void FixedUpdate()
    {
        moveDelta = new Vector2(joystick.Horizontal * moveSpeed, joystick.Vertical * moveSpeed);

        transform.Translate(moveDelta * Time.deltaTime);


        Debug.Log("Horizontal:" + joystick.Horizontal);
        Debug.Log("Vertical:" + joystick.Vertical);
    }
}