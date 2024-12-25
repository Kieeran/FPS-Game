using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public static EditPlayerName Instance { get; private set; }
    public event EventHandler OnNameChanged;
    [SerializeField] private TextMeshProUGUI playerNameText;

    public string playerName = "PlayerName";

    private void Awake()
    {
        Instance = this;

        // GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     UI_InputWindow.Show_Static(
        //         "Player Name",
        //         playerName,
        //         "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-",
        //         20,
        //         onCancel,
        //         onOk
        //     );
        // });
    }

    private void Start()
    {
        OnNameChanged += EditPlayerName_OnNameChanged;

        playerNameText.text = playerName;
    }

    // private void onCancel()
    // {
    //     // Cancel
    // }

    // private void onOk(string newName)
    // {
    //     playerName = newName;

    //     playerNameText.text = playerName;

    //     OnNameChanged?.Invoke(this, EventArgs.Empty);
    // }

    public void ReadPlayerNameInputField(string s)
    {
        playerName = s;
        playerNameText.text = playerName;

        OnNameChanged?.Invoke(this, EventArgs.Empty);
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
    {
        LobbyManager.Instance.UpdatePlayerName(playerName);
    }
}