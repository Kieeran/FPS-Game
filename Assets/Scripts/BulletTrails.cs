using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletTrails : MonoBehaviour
{
    private float delayTime = 1f;
    private float counter = 0f;

    public InputActionAsset controller;
    private InputAction rotateYLeft;
    private InputAction rotateYRight;
    private InputAction rotateXLeft;
    private InputAction rotateXRight;

    public bool Automatic;
    private float CurrentCoolDown;
    [SerializeField]
    private float FireCoolDown;

    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    private Quaternion targetRotation;
    float targetAngleX;
    float targetAngleY;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        counter += Time.deltaTime * fireRate;
        if (counter <= delayTime) return;
        counter = 0f;
    }

    private void OnShoot()
    {
        if (Automatic)
        {
            if (Input.GetKey(KeyCode.V))
            {
                if (CurrentCoolDown <= 0f)
                {
                    CurrentCoolDown = FireCoolDown;
                    //
                    Bullet bullet = BulletManager.Instance.GetBullet();
                    bullet.transform.position = bulletSpawnPoint.position;

                    Vector3 forceDirection = transform.forward * speed;

                    bullet.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
                    bullet.StartCountingToDisappear();
                }
            }
        }

        else
        {
            if (Input.GetKey(KeyCode.V))
            {
                if (CurrentCoolDown <= 0f)
                {
                    CurrentCoolDown = FireCoolDown;
                    //
                    Bullet bullet = BulletManager.Instance.GetBullet();
                    bullet.transform.position = bulletSpawnPoint.position;

                    Vector3 forceDirection = transform.forward * speed;

                    bullet.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
                    bullet.StartCountingToDisappear();
                }
            }
        }

        CurrentCoolDown -= Time.deltaTime;
    }

    //private void OnEnable()
    //{
    //    rotateYLeft = controller.FindAction("RotateYLeft");
    //    rotateYRight = controller.FindAction("RotateYRight");
    //    rotateXLeft = controller.FindAction("RotateXLeft");
    //    rotateXRight = controller.FindAction("RotateXRight");

    //    rotateYLeft.Enable();
    //    rotateYRight.Enable();
    //    rotateXLeft.Enable();
    //    rotateXRight.Enable();

    //    rotateYLeft.performed += OnRotateYLeft;
    //    rotateYRight.performed += OnRotateYRight;
    //    rotateXLeft.performed += OnRotateXLeft;
    //    rotateXRight.performed += OnRotateXRight;
    //}

    //private void OnDisable()
    //{
    //    rotateYLeft.Disable();
    //    rotateYRight.Disable();
    //    rotateXLeft.Disable();
    //    rotateXRight.Disable();

    //    rotateYLeft.performed -= OnRotateYLeft;
    //    rotateYRight.performed -= OnRotateYRight;
    //    rotateXLeft.performed -= OnRotateXLeft;
    //    rotateXRight.performed -= OnRotateXRight;
    //}

    //private void OnRotateYLeft(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context);
    //    targetAngleY = transform.eulerAngles.y - rotationSpeed * Time.deltaTime;
    //}

    //private void OnRotateYRight(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context);
    //    targetAngleY = transform.eulerAngles.y + rotationSpeed * Time.deltaTime;
    //}

    //private void OnRotateXLeft(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context);
    //    targetAngleX = transform.eulerAngles.x - rotationSpeed * Time.deltaTime;
    //}

    //private void OnRotateXRight(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context);
    //    targetAngleX = transform.eulerAngles.x + rotationSpeed * Time.deltaTime;
    //}

    private void FixedUpdate()
    {
        /// Control turret
        if (Input.GetKey(KeyCode.J))
            targetAngleY = transform.eulerAngles.y - rotationSpeed * Time.deltaTime;

        else if (Input.GetKey(KeyCode.L))
            targetAngleY = transform.eulerAngles.y + rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.I))
            targetAngleX = transform.eulerAngles.x - rotationSpeed * Time.deltaTime;

        else if (Input.GetKey(KeyCode.K))
            targetAngleX = transform.eulerAngles.x + rotationSpeed * Time.deltaTime;

        targetRotation = Quaternion.Euler(targetAngleX, targetAngleY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        OnShoot();
    }
}