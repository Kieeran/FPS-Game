using PlayerAssets;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private Rigidbody rb;

    public float groundDrag;

    // [Header("Crouching")]
    // public float crouchSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    //public LayerMask Ground;
    private bool isGrounded;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool isReadyToJump;

    public Transform orientation;

    private Vector2 moveInput;

    private Vector3 moveDirection;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }


    private void Start()
    {
        isReadyToJump = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);

        SpeedControl();
        StateHandler();

        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        if (IsOwner == false) return;

        MovePlayer();
    }

    // private void MyInput()
    // {
    //     moveInput = new Vector2(
    //         PlayerInput.Instance.GetMoveInput().x,
    //         PlayerInput.Instance.GetMoveInput().y);

    //     isJumped = PlayerInput.Instance.GetIsJumped();

    //     if (isJumped && isGrounded)
    //     {
    //         Jump();
    //         isGrounded = false;
    //         PlayerInput.Instance.GetPlayerAssetsInputs().jump = false;
    //     }

    //     if (Input.GetKeyDown(crouchKey))
    //     {
    //        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    //        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    //     }

    //     if (Input.GetKeyUp(crouchKey))
    //     {
    //        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    //        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    //     }

    //     Debug.Log("Space key: " + Input.GetKey(jumpKey));
    // }

    private void StateHandler()
    {
        if (isGrounded)
        {
            if (playerAssetsInputs.sprint)
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
            }
            else
            {
                state = MovementState.walking;
                moveSpeed = walkSpeed;
            }
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
            state = MovementState.air;
    }

    private void MovePlayer()
    {
        moveInput = playerAssetsInputs.move;

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (isGrounded)
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        //Debug.Log("Process jump");
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     isGrounded = true;
    // }
}