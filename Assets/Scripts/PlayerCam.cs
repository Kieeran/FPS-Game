using Unity.Netcode;
using UnityEngine;

public class PlayerCam : NetworkBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public GameObject _camera;

    private float xRotation;
    private float yRotation;

    private Vector2 mousePos;

    private bool toggleOut;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        toggleOut = false;

        if (IsOwner == true) _camera.gameObject.SetActive(true);
    }

    // private void Start()
    // {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    //     toggleOut = false;
    // }

    private void Update()
    {
        if (IsOwner == false) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            toggleOut = !toggleOut;
        }

        if (toggleOut == false)
        {
            mousePos.x = Input.GetAxis("Mouse X");
            mousePos.y = Input.GetAxis("Mouse Y");

            mousePos.x *= Time.deltaTime * sensX;
            mousePos.y *= Time.deltaTime * sensY;

            yRotation += mousePos.x;
            xRotation -= mousePos.y;

            xRotation = Mathf.Clamp(xRotation, -80f, 50f);

            orientation.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            //transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}