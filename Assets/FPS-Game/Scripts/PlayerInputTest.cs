using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputTest : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private Rigidbody rb;
    private Vector3 movement;

    private void FixedUpdate()
    {
        rb.AddForce(movement * speed * Time.deltaTime * 100);
    }

    public void OnMove(InputValue value)
    {

        Vector2 _movement = value.Get<Vector2>();
        movement = new Vector3(_movement.x, 0, _movement.y);

    }

    public void OnLook(InputValue value)
    {

        Vector2 vector2 = value.Get<Vector2>();
        Debug.Log(vector2.x + " " + vector2.y);
    }
}