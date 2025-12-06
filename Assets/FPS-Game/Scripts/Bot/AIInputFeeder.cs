using System;
using UnityEngine;

public class AIInputFeeder : PlayerBehaviour
{
    public Vector3 moveDir;
    public Action<Vector3> OnMove;

    void Start()
    {
        OnMove += (val) =>
        {
            moveDir = val;
        };
    }
}