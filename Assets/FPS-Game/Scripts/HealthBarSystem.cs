using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarSystem : MonoBehaviour
{
    public static HealthBarSystem Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    private Queue<Canvas> healthCanvas;
    private List<Canvas> _healthCanvas;
    public Canvas healthBarPrefab;
    public Camera playerCamera;


    private void Start()
    {
        InitPooling();
    }

    private void Update()
    {
        for (int i = 0; i < _healthCanvas.Count; i++)
        {
            _healthCanvas[i].transform.LookAt(playerCamera.transform);
        }
    }

    private void InitPooling()
    {
        healthCanvas = new Queue<Canvas>();
        _healthCanvas = new List<Canvas>();

        for (int i = 0; i < 4; i++)
        {
            Canvas canvas = Instantiate(healthBarPrefab);
            canvas.gameObject.SetActive(false);
            healthCanvas.Enqueue(canvas);
        }
    }

    public Canvas ShowHealthBar()
    {
        Canvas canvas = healthCanvas.Dequeue();
        _healthCanvas.Add(canvas);
        //Invoke("HideHealthBar", 2f);
        return canvas;
    }

    //private void HideHealthBar()
    //{

    //}

    //public Canvas Get
}