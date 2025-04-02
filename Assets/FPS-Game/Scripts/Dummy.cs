using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private int hp;

    private bool isDestroy;

    public bool GetIsDestroy() { return isDestroy; }

    private int startHP;

    private void Start()
    {
        isDestroy = false;

        startHP = hp;
    }

    public void GetDamage(int damage)
    {
        if (isDestroy == true) return;

        hp -= damage;

        if (hp <= 0)
        {
            gameObject.SetActive(false);
            isDestroy = true;
        }
    }

    public void ResetDummy()
    {
        isDestroy = false;
        hp = startHP;
    }
}