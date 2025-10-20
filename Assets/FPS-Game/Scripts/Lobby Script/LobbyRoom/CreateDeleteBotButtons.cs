using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateDeleteBotButtons : MonoBehaviour
{
    [SerializeField] Button _createBotButton;
    [SerializeField] Button _deleteBotButton;

    public Action OnCreateBot;
    public Action OnDeleteBot;

    void Awake()
    {
        _createBotButton.onClick.AddListener(() =>
        {
            OnCreateBot?.Invoke();
        });

        _deleteBotButton.onClick.AddListener(() =>
        {
            OnDeleteBot?.Invoke();
        });
    }
}