using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public static EditPlayerName Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI playerNameText;

    private string playerName;

    public const string DEFAULT_PLAYER_NAME = "PlayerName";

    public string GetPlayerName() { return playerName; }
    public void SetPlayerName(string s) { playerName = s; }

    private void Awake()
    {
        Instance = this;

        playerName = DEFAULT_PLAYER_NAME;

        if (LobbyManager.Instance.GetPlayerName() != null)
        {
            playerName = LobbyManager.Instance.GetPlayerName();
        }
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