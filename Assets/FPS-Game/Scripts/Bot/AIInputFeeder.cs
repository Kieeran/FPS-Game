using System;
using UnityEngine;

public class AIInputFeeder : PlayerBehaviour
{
    public Vector3 moveDir;
    public float targetPitch;
    public Action<Vector3> OnMove;
    public Action<float> OnLook;

    void Start()
    {
        OnMove += (val) =>
        {
            moveDir = val;
        };

        OnLook += (val) =>
        {
            targetPitch = val;
        };
    }
}