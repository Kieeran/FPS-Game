using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;
using TMPro;
using System;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance { get; private set; }
    [SerializeField] private SlotPlayer prefabSlotPlayer;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnOutLobby;
    }

    private void LobbyManager_OnOutLobby(object sender, System.EventArgs e)
    {
        LobbyManager.joinedLobby = null;

        LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnOutLobby;
        LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnOutLobby;

        GameSceneManager.Instance.LoadPreviousScene();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        if (LobbyManager.Instance.GetJoinedLobby() == null)
        {
            Debug.Log("JoinedLobby null!");
            return;
        }

        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

        ClearSlotPlayers();

        for (int i = 0; i < lobby.Players.Count; i++)
        {
            SlotPlayer slotPlayer = Instantiate(prefabSlotPlayer);

            Transform slot = gameObject.transform.GetChild(i);

            slotPlayer.transform.SetParent(slot.transform);
            slotPlayer.transform.localPosition = prefabSlotPlayer.transform.localPosition;
            slotPlayer.transform.localRotation = prefabSlotPlayer.transform.localRotation;
            slotPlayer.transform.localScale = prefabSlotPlayer.transform.localScale;

            // Don't allow kick self
            slotPlayer.SetKickPlayerButtonDisable(
                LobbyManager.Instance.IsLobbyHost() &&
                lobby.Players[i].Id != AuthenticationService.Instance.PlayerId
            );

            slotPlayer.UpdatePlayer(lobby.Players[i]);

            slotPlayer.GetSlotCanvas().worldCamera = mainCamera;
        }
    }

    private void ClearSlotPlayers()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.childCount > 0)
                Destroy(child.transform.GetChild(0).gameObject);
        }
    }

    // private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    // {
    //     LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
    //     LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
    //     LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;

    //     GameSceneManager.Instance.LoadPreviousScene();
    // }
}