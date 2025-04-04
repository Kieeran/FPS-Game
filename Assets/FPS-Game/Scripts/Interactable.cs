using UnityEngine;

public class Interactable : MonoBehaviour
{
    Outline _outline;

    void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public void EnableOutline()
    {
        _outline.enabled = true;
    }

    public void DisableOutline()
    {
        _outline.enabled = false;
    }
}
