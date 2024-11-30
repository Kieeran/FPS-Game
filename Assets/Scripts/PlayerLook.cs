using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public Camera _camera;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector2 mousePos;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

        //Debug.Log(mouseX + "; " + mouseY);

        //yRotation += mouseX;
        //xRotation -= mouseY;

        mousePos.x *= Time.deltaTime * xSensitivity;
        mousePos.y *= Time.deltaTime * ySensitivity;

        yRotation += mousePos.x;
        xRotation -= mousePos.y;

        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        _camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void OnLook(InputValue value)
    {
        mousePos = value.Get<Vector2>();
    }

    //public void ProcessLook(Vector2 input)
    //{
    //    float mouseX = input.x;
    //    float mouseY = input.y;

    //    xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
    //    xRotation = Mathf.Clamp(xRotation, -80f, 80f);
    //    _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    //    transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    //}
}