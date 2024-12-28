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
    public TextMeshProUGUI lobbyCode;
    public SlotPlayer prefabSlotPlayer;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }
    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        //if (TestLobby.Instance = null) return;

        //Lobby lobby = TestLobby.Instance.GetJoinedLobby();

        if (LobbyManager.Instance.GetJoinedLobby() == null) return;

        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

        // show lobby code
        lobbyCode.text = lobby.LobbyCode;

        //Debug.Log("Update lobby with code " + lobby.LobbyCode);

        ClearSlotPlayers();
        //Debug.Log(lobbyCode.text);
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

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;

        GameSceneManager.Instance.LoadPreviousScene();
    }
}