using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineCosineMovement : MonoBehaviour
{
    private float speedCurve;
    private float startX;
    private float startY;
    public float amplitude;
    public float speed;

    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    private void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;

        //startX = 2f;
        //startY = 2f;
    }

    private void Update()
    {
        speedCurve += Time.deltaTime + speed;

        //Debug.Log("curveSin: " + curveSin);
        //Debug.Log("speedCurve: " + speedCurve);

        transform.position = new Vector3(startX + amplitude * curveCos, startY + amplitude * curveSin, transform.position.z);
    }
}
