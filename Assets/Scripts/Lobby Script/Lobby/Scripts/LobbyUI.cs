using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    private Player player;

    // [SerializeField] private Transform playerSingleTemplate;
    // [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    // [SerializeField] private TextMeshProUGUI gameModeText;
    // [SerializeField] private Button changeMarineButton;
    // [SerializeField] private Button changeNinjaButton;
    // [SerializeField] private Button changeZombieButton;
    [SerializeField] private Button leaveLobbyButton;
    // [SerializeField] private Button kickPlayerButton;
    // [SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI lobbyCode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // playerSingleTemplate.gameObject.SetActive(false);

        // changeMarineButton.onClick.AddListener(() =>
        // {
        //     LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Marine);
        // });
        // changeNinjaButton.onClick.AddListener(() =>
        // {
        //     LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Ninja);
        // });
        // changeZombieButton.onClick.AddListener(() =>
        // {
        //     LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.Zombie);
        // });

        leaveLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
        });

        // kickPlayerButton.onClick.AddListener(() =>
        // {
        //     LobbyManager.Instance.KickPlayer(player.Id);
        // });

        // changeGameModeButton.onClick.AddListener(() =>
        // {
        //     LobbyManager.Instance.ChangeGameMode();
        // });

        startGameButton.onClick.AddListener(() =>
        {
            //SceneManager.LoadScene("Playground");
            GameSceneManager.Instance.LoadNextScene();
            LobbyManager.Instance.StartGame();
        });
    }

    private void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnGameStarted += LobbyManager_OnGameStarted;
    }

    private void LobbyManager_OnGameStarted(object sender, System.EventArgs e)
    {
        //LobbyManager.joinedLobby = null;
        // GameSceneManager.Instance.LoadNextScene();
        // ChatCanvasUI.Instance.Show();
    }

    private void LobbyManager_OnOutLobby(object sender, System.EventArgs e)
    {
        LobbyManager.joinedLobby = null;

        LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnGameStarted -= LobbyManager_OnGameStarted;
    }

    // private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e)
    // {
    //     LobbyManager.joinedLobby = null;
    // }

    // private void LobbyManager_OnKickedFromLobby(object sender, System.EventArgs e)
    // {
    //     LobbyManager.joinedLobby = null;

    //     // Show Lobby Create UI when kicked
    //     LobbyListUI.Instance.Show();
    // }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby)
    {
        Debug.Log(lobby);

        ShowLobbyCode();
        startGameButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        // gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;
    }

    private void ShowLobbyCode()
    {
        lobbyCode.text = LobbyManager.Instance.GetJoinedLobbyCode();
    }

    // public void ClearLobby()
    // {
    //     if (GameManager.currentIndex == 2) return;

    //     foreach (Transform child in container)
    //     {
    //         if (child == playerSingleTemplate) continue;
    //         Destroy(child.gameObject);
    //     }
    // }
}