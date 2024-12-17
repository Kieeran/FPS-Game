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

    public void Setup(string name)
    {
        nameText.text = name.ToString();
        killsText.text = "0";
        deathsText.text = "0";
    }

    public void SetKills(int kills)
    {
        killsText.text = kills.ToString();
    }

    public void SetDeaths(int deaths)
    {
        deathsText.text = deaths.ToString();
    }
}
