using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;

    void Start()
    {
        // killsText.text = "0";
        // deathsText.text = "0";
    }

    public void Setup(string name, int killCount, int deathCount)
    {
        nameText.text = name.ToString();
        killsText.text = killCount.ToString();
        deathsText.text = deathCount.ToString();
    }
}
