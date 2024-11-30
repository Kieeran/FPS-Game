using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private float maxHealth = 100f;
    private float currentHealth;
    private Image health;
    private Canvas healthBar;

    public float GetCurrentHealth() { return currentHealth; }

    private void Start()
    {
        currentHealth = maxHealth;
        Invoke("CreateHealthBar", 0.2f);
    }

    private void CreateHealthBar()
    {
        healthBar = HealthBarSystem.Instance.ShowHealthBar();
        healthBar.transform.SetParent(transform);
        healthBar.transform.localPosition = Vector3.zero + new Vector3(0, 1.31f, 0);

        var target = healthBar.transform.Find("HealthBar/Background/Bar/Health");
        health = target.GetComponent<Image>();
        //Debug.Log(health);
    }
    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            return;
        }
        healthBar.gameObject.SetActive(true);

        health.fillAmount = (float)currentHealth / 100f;
        Invoke("HideHealthBar", 2f);
    }

    private void HideHealthBar()
    {
        healthBar.gameObject.SetActive(false);
    }
}