using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] string areaName;

    void Awake()
    {
        areaName = gameObject.name;
    }
}