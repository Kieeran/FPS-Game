using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    public List<Area> allAreas { get; private set; } = new();
    [SerializeField] Transform areasContainer;

    public void InitAreas(Transform tf)
    {
        areasContainer = tf;

        foreach (Transform t in areasContainer)
        {
            if (t.TryGetComponent<Area>(out var area))
            {
                allAreas.Add(area);
            }
        }
    }
}
