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
}