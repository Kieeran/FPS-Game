using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image HealthUI;

    public void UpdatePlayerHealthBar(float amount)
    {
        HealthUI.fillAmount = amount;
    }

    //private void Start()
    //{
    //    SetHealth(1f);
    //    health = 100f;
    //}

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.J))
    //    {
    //        health -= 0.1f;
    //        if (health < 0)
    //            health = 0;
    //    }

    //    else if (Input.GetKey(KeyCode.K))
    //    {
    //        health += 0.1f;
    //        if (health > 100)
    //            health = 100;
    //    }
    //    SetHealth((float)health / 100f);
    //}

    //private void SetHealth(float healthNormalized)
    //{
    //    healthBar.fillAmount = healthNormalized;
    //}
}