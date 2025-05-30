using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public List<Renderer> headParts;

    public void HideHead()
    {
        foreach (var part in headParts)
        {
            part.enabled = false;
        }
    }
}
