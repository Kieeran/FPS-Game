using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class _PlayerMovement : MonoBehaviour
{
    //public Rigidbody rb;
    //public float jumpForce;
    //public float moveSpeed;
    //public float extraGravity;

    //private bool isGrounded;
    //private Vector2 moveDirection;
    //private bool isJumped = false;

    //private void Awake()
    //{

    //}

    //private void Start()
    //{
    //    isGrounded = true;
    //}

    //private void Update()
    //{
    //    //Debug.Log(isJumped);
    //    if (isJumped)
    //        Jump();
    //}

    //public void OnMove(InputValue value)
    //{
    //    moveDirection = value.Get<Vector2>();
    //}

    //public void OnJump(InputValue value)
    //{
    //    if (value.isPressed == true)
    //        isJumped = true;
    //    else
    //        isJumped = false;
    //}

    //private void LateUpdate()
    //{
    //}

    //private void FixedUpdate()
    //{
    //    rb.velocity = new Vector3(
    //    moveDirection.x * moveSpeed * Time.deltaTime,
    //    rb.velocity.y + extraGravity,
    //    moveDirection.y * moveSpeed * Time.deltaTime
    //    );
    //}

    //private void Jump()
    //{
    //    if (isGrounded)
    //    {
    //        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //        isGrounded = false;
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Floor"))
    //    {
    //        isGrounded = true;
    //    }
    //}
}