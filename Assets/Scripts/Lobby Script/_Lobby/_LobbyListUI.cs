using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class _LobbyListUI : MonoBehaviour
{
    public static _LobbyListUI Instance { get; private set; }

    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button joinButton;


    private void Awake()
    {
        Instance = this;

        lobbySingleTemplate.gameObject.SetActive(false);

        refreshButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.RefreshLobbyList();
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            LobbyCreateUI.Instance.Show();
        });

        joinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
        });

        // Hide();
    }

    private void Start()
    {
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnKickedFromLobby(object sender, EventArgs e)
    {
        Show();
    }

    // private void LobbyManager_OnJoinedLobbyByCode(object sender, LobbyManager.LobbyEventArgs e) {
    //     Hide();
    // }
    
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in container)
        {
            if (child == lobbySingleTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    // private void RefreshButtonClick()
    // {
    //     LobbyManager.Instance.RefreshLobbyList();
    // }

    // private void CreateLobbyButtonClick()
    // {
    //     LobbyCreateUI.Instance.Show();
    // }

    // private void JoinLobbyWithCode()
    // {
    //     LobbyManager.Instance.JoinLobbyByCode(lobbyCodeInput.text);
    // }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

}