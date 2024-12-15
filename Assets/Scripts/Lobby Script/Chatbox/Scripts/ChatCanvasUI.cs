using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChatCanvasUI : MonoBehaviour
{
    public static ChatCanvasUI Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    void Awake() {
        Instance = this;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}
