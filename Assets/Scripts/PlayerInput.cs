using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;

    public PlayerAssetsInputs GetPlayerAssetsInputs() { return playerAssetsInputs; }

    private Vector2 mousePos;
    private Vector2 moveInput;
    private bool isJumped;
    private bool isReloaded;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnReload(InputValue value)
    {
        if (value != null)
        {
            if (value.isPressed == true)
            {
                isReloaded = true;
            }

            else
            {
                isReloaded = false;
            }
        }
    }

    public void OnJump(InputValue value)
    {
        if (value != null)
        {
            if (value.isPressed == true)
                isJumped = true;
            else
                isJumped = false;
        }
    }

    public void OnLook(InputValue value)
    {
        if (value != null)
        {
            mousePos = value.Get<Vector2>();
        }
    }

    //public bool GetIsJumped() { return isJumped; }
    public bool GetIsJumped() { return playerAssetsInputs.jump; }
    //public bool GetIsReloaded() { return isReloaded; }
    public bool GetIsReloaded() { return playerAssetsInputs.reload; }
    //public Vector2 GetMoveInput() { return moveInput; }
    public Vector2 GetMoveInput() { return playerAssetsInputs.move; }

    //public Vector2 GetMousePos() { return mousePos; }
    public Vector2 GetMousePos() { return playerAssetsInputs.look; }
}