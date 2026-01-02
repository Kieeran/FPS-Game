using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField] string zoneName;

    void Awake()
    {
        zoneName = gameObject.name;
    }
}