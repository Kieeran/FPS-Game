using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    [SerializeField] private Image health;

    private void Start()
    {
        health.fillAmount = 1;
    }

    public void UpdateHealth(float amount)
    {
        health.fillAmount -= amount;
        if (health.fillAmount == 0)
        {
            health.fillAmount = 1;
        }
    }
}