using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    private float xRotation;
    private float yRotation;

    public Transform orientation;

    private Vector2 mousePos;

    private bool toggleOut;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        toggleOut = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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
            xRotation += mousePos.y;

            xRotation = Mathf.Clamp(xRotation, -80f, 50f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}