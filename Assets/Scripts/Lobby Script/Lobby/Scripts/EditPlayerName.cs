using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public static EditPlayerName Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI playerNameText;

    public string playerName = "PlayerName";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerNameText.text = playerName;
    }

    public void ReadPlayerNameInputField(string s)
    {
        playerName = s;
        playerNameText.text = playerName;

        UpdatePlayerName();
    }

    private void UpdatePlayerName()
    {
        LobbyManager.Instance.UpdatePlayerName(playerName);
    }
}