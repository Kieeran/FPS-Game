using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    public static LobbyCreateUI Instance { get; private set; }

    [SerializeField] private Button createButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_InputField maxPlayersInputField;
    [SerializeField] private Toggle isPrivateToggle;
    [SerializeField] private Button returnButton;

    public const string DEFAULT_LOBBY_NAME = "Default name";
    public const int DEFAULT_MAX_PLAYERS = 2;
    public const bool DEFAULT_IS_PRIVATE = false;

    // [SerializeField] private Button lobbyNameButton;
    // [SerializeField] private Button publicPrivateButton;
    // [SerializeField] private Button maxPlayersButton;
    // [SerializeField] private Button gameModeButton;
    //[SerializeField] private TextMeshProUGUI lobbyNameText;
    // [SerializeField] private TextMeshProUGUI publicPrivateText;
    // [SerializeField] private TextMeshProUGUI maxPlayersText;
    // [SerializeField] private TextMeshProUGUI gameModeText;

    private string lobbyName;
    private bool isPrivate;
    private int maxPlayers;
    // private LobbyManager.GameMode gameMode;

    private void Awake()
    {
        Instance = this;

        returnButton.onClick.AddListener(() =>
        {
            Hide();
            LobbyListUI.Instance.Show();
        });

        createButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate/*,gameMode*/);

            //Hide();
        });

        // lobbyNameButton.onClick.AddListener(() =>
        // {
        //     UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
        //     () =>
        //     {
        //         // Cancel
        //     },
        //     (string lobbyName) =>
        //     {
        //         this.lobbyName = lobbyName;
        //         UpdateText();
        //     });
        // });

        // publicPrivateButton.onClick.AddListener(() =>
        // {
        //     isPrivate = !isPrivate;
        //     UpdateText();
        // });

        // maxPlayersButton.onClick.AddListener(() =>
        // {
        //     UI_InputWindow.Show_Static("Max Players", maxPlayers,
        //     () =>
        //     {
        //         // Cancel
        //     },
        //     (int maxPlayers) =>
        //     {
        //         this.maxPlayers = maxPlayers;
        //         UpdateText();
        //     });
        // });

        // gameModeButton.onClick.AddListener(() =>
        // {
        //     switch (gameMode)
        //     {
        //         default:
        //         case LobbyManager.GameMode.PvE:
        //             gameMode = LobbyManager.GameMode.PvP;
        //             break;
        //         case LobbyManager.GameMode.PvP:
        //             gameMode = LobbyManager.GameMode.PvE;
        //             break;
        //     }
        //     UpdateText();
        // });

        Hide();
    }

    public void ReadLobbyName(string s)
    {
        lobbyName = s;
        if (s == "")
        {
            lobbyName = DEFAULT_LOBBY_NAME;
        }
        UpdateText();
    }

    public void ReadMaxPlayers(string s)
    {
        int playerNum = int.Parse(s);
        maxPlayers = playerNum;

        if (playerNum < 1 && playerNum > 5) maxPlayers = DEFAULT_MAX_PLAYERS;

        UpdateText();
    }

    public void ReadIsPrivate(bool b)
    {
        isPrivate = b;
    }

    private void UpdateText()
    {
        lobbyNameInputField.text = lobbyName;
        maxPlayersInputField.text = maxPlayers.ToString();
        //gameModeText.text = gameMode.ToString();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        lobbyName = DEFAULT_LOBBY_NAME;
        isPrivate = DEFAULT_IS_PRIVATE;
        maxPlayers = DEFAULT_MAX_PLAYERS;
        // gameMode = LobbyManager.GameMode.PvE;

        UpdateText();
    }
}