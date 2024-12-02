using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCam : NetworkBehaviour
{
    public float sensX;
    public float sensY;

    private float xRotation;
    private float yRotation;

    public Transform orientation;

    private Vector2 mousePos;

    private bool toggleOut;

    private Camera playerCamera;

    // public override void OnNetworkSpawn() {

    // }

    private void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();

        if (!IsOwner)
        {
            // Disable the camera and audio listener for non-owners
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                var audioListener = playerCamera.GetComponent<AudioListener>();
                if (audioListener != null)
                    audioListener.enabled = false;
            }
        }
        else
        {
            // Enable cursor lock only for the owner
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            toggleOut = false;
        }
    }

    private void Update()
    {
        if (IsOwner) {
            if (Input.GetKey(KeyCode.Q))
            {
                toggleOut = !toggleOut;
            }

            if (toggleOut == false)
            {
                mousePos = PlayerInput.Instance.GetMousePos();

                mousePos.x *= Time.deltaTime * sensX;
                mousePos.y *= Time.deltaTime * sensY;

                //Debug.Log(mouseX + "; " + mouseY);

                yRotation += mousePos.x;
                xRotation -= mousePos.y;

                xRotation = Mathf.Clamp(xRotation, -80f, 50f);

                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
                orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
    }
}