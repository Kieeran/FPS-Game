using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public static EditPlayerName Instance { get; private set; }
    public event EventHandler OnNameChanged;
    [SerializeField] private TextMeshProUGUI playerNameText;

    private string playerName = "PlayerName";

    private void Awake()
    {
        Instance = this;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            UI_InputWindow.Show_Static(
                "Player Name",
                playerName,
                "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-",
                20,
                onCancel,
                onOk
            );
        });

        if (LobbyManager.Instance != null)
        {
            playerName = LobbyManager.Instance.GetPlayerName();
        }
        playerNameText.text = playerName;
    }

    private void onCancel()
    {
        // Cancel
    }

    private void onOk(string newName)
    {
        playerName = newName;

        playerNameText.text = playerName;

        OnNameChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        // if (AuthenticateUI.editNameFlag == 0) 
        OnNameChanged += EditPlayerName_OnNameChanged;
        // playerName = GetPlayerName();

        if (GameSceneManager.Instance != null)
        {
            playerName = GameSceneManager.Instance.GetPlayerName();
            playerNameText.text = playerName;
        }
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
    {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
        //GameSceneManager.Instance.playerName = playerName;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
}