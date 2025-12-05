using System;
using UnityEngine;

public class AIInputFeeder : PlayerBehaviour
{
    public Action<Vector2> OnMove;

    void Start()
    {
        OnMove += (val) =>
        {
            if (PlayerRoot.PlayerAssetsInputs == null || val == null)
                return;
            PlayerRoot.PlayerAssetsInputs.MoveInput(val);
        };
    }
}