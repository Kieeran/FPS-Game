using System;
using UnityEngine;

public class AIInputFeeder : PlayerBehaviour
{
    public Vector3 moveDir;
    public Vector3 lookEuler;
    public Action<Vector3> OnMove;
    public Action<Vector3> OnLook;

    void Start()
    {
        OnMove += (val) =>
        {
            moveDir = val;
        };

        OnLook += (val) =>
        {
            lookEuler = val;
        };
    }
}