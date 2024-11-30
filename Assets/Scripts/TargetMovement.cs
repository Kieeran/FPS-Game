using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    //private float counter;
    //private float delay = 0.05f;
    public float moveX;
    private void Update()
    {
        //counter += Time.deltaTime;
        //if (counter < delay) return;
        //counter = 0;

        transform.position = new Vector3(
            transform.position.x + moveX,
            transform.position.y,
            transform.position.z
            );
        if (!(transform.position.x <= 16.5f && transform.position.x >= 9.5f))
        {
            moveX *= -1;
        }
    }
}
