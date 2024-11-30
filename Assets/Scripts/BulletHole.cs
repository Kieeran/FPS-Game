using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public void StartCounting()
    {
        Invoke("DisableSelf", 7f);
    }

    private void DisableSelf()
    {
        BulletHoleManager.Instance.ReturnBulletHole(this);
    }

}