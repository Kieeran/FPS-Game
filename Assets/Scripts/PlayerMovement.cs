using PlayerAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public PlayerAssetsInputs playerAssetsInputs;

    //public float groundDrag;

    //[Header("Crouching")]
    //public float crouchSpeed;
    //public float crouchYScale;
    //private float startYScale;

    //[Header("Ground Check")]
    //public float playerHeight;
    //public LayerMask Ground;
    private bool isGrounded;

    //[Header("keybinds")]
    //public KeyCode jumpKey = KeyCode.Space;
    //public KeyCode sprintKey = KeyCode.LeftShift;
    //public KeyCode crouchKey = KeyCode.LeftControl;

    public float jumpForce;
    //public float jumpCooldown;
    //public float airMultiplier;
    //private bool isReadyToJump;

    public Transform orientation;

    private Vector2 moveInput;
    private bool isJumped;

    private Vector3 moveDirection;

    public Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //isReadyToJump = true;
        //startYScale = transform.localScale.y;
    }

    private void Update()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        //MyInput();
        //SpeedControl();
        //StateHandler();

        //if (isGrounded)
        //    rb.drag = groundDrag;
        //else
        //    rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        moveInput = new Vector2(
            PlayerInput.Instance.GetMoveInput().x,
            PlayerInput.Instance.GetMoveInput().y);

        isJumped = PlayerInput.Instance.GetIsJumped();

        if (isJumped && isGrounded)
        {
            Jump();
            isGrounded = false;
            PlayerInput.Instance.GetPlayerAssetsInputs().jump = false;
        }

        //if (Input.GetKeyDown(crouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        //}

        //if (Input.GetKeyUp(crouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        //    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        //}

        //Debug.Log("Space key: " + Input.GetKey(jumpKey));
    }

    private void StateHandler()
    {
        if (isGrounded)
        {
            //if (Input.GetKey(sprintKey))
            //{
            //    state = MovementState.sprinting;
            //    moveSpeed = sprintSpeed;
            //}
            //else if (Input.GetKey(crouchKey))
            //{
            //    state = MovementState.crouching;
            //    //moveSpeed = crouchSpeed;
            //}
            //else
            //{
            //    state = MovementState.walking;
            //    moveSpeed = walkSpeed;
            //}
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        else
            state = MovementState.air;
    }

    private void MovePlayer()
    {
        //moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        Vector2 moveInput = playerAssetsInputs.move;

        moveDirection.x = moveInput.x;
        moveDirection.z = moveInput.y;

        Debug.Log(moveDirection);

        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        //if (isGrounded)
        //    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //else
        //    rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
        {
            isGrounded = true;
        }
    }
}