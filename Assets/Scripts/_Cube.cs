using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class _Cube : MonoBehaviour
{
    public float counter;
    public float speed;

    private void Update()
    {
        counter -= Time.deltaTime;
        if (counter >= 0f) return;
        counter = 0.1f;

        GetComponent<Rigidbody>().velocity = new Vector3(speed, 0f, 0f);

        if (transform.position.x > 30f)
        {
            transform.position = new Vector3(30f, 0f, 0f);
            speed *= -1;
        }

        if (transform.position.x < 0f)
        {
            transform.position = new Vector3(0f, 0f, 0f);
            speed *= -1;
        }
    }
}