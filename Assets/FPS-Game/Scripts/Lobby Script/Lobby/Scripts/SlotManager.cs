using UnityEngine;
using System;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using TMPro;

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

    private void OnDestroy()
    {
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
            LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        }
    }

    private void LobbyManager_OnOutLobby(object sender, EventArgs e)
    {
        LobbyManager.joinedLobby = null;

        // LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
        // LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
        // LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnOutLobby;
        // LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnOutLobby;

        // GameSceneManager.Instance.LoadPreviousScene();
        GameSceneManager.Instance.LoadScene("Lobby List");
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e)
    {
        if (this == null || !gameObject.activeInHierarchy)
        {
            return;
        }

        if (LobbyManager.Instance.GetJoinedLobby() == null)
        {
            Debug.Log("JoinedLobby null!");
            return;
        }

        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();

        // Lobby luôn cập nhật sau mỗi ns nên cần phải clear các player trước để thêm các player mới vào
        ClearSlotPlayers();

        // Thêm mới các player trong lobby
        int i;
        for (i = 0; i < lobby.Players.Count; i++)
        {
            SlotPlayer slotPlayer = CreateSlotPlayerAt(i);

            // Don't allow kick self
            slotPlayer.SetKickPlayerButtonDisable(
                LobbyManager.Instance.IsLobbyHost() &&
                lobby.Players[i].Id != AuthenticationService.Instance.PlayerId
            );

            slotPlayer.UpdatePlayer(lobby.Players[i]);

            slotPlayer.GetSlotCanvas().worldCamera = mainCamera;
        }

        // Thêm các bot vào lobby, kiểm tra điều kiện đảm bảo không thêm bot quá 5 hoặc khi lobby đã đầy
        if (i < 5)
        {
            int botNum = LobbyManager.Instance.GetBotNum();
            for (int j = i; j < botNum + i; j++)
            {
                SlotPlayer slotPlayer = CreateSlotPlayerAt(j);
                slotPlayer.UpdatePlayerName("Bot");
            }
        }
        // Debug.Log(LobbyManager.Instance.GetBotNum());
    }

    private void ClearSlotPlayers()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.childCount > 0)
                Destroy(child.transform.GetChild(0).gameObject);
        }
    }

    // Tạo một slot player mới (về mặt hiển thị) ở index (vị trí của player trên các đĩa)
    SlotPlayer CreateSlotPlayerAt(int index)
    {
        SlotPlayer slotPlayer = Instantiate(prefabSlotPlayer);

        Transform slot = gameObject.transform.GetChild(index);

        slotPlayer.transform.SetParent(slot.transform);
        slotPlayer.transform.SetLocalPositionAndRotation(
            prefabSlotPlayer.transform.localPosition,
            prefabSlotPlayer.transform.localRotation
        );
        slotPlayer.transform.localScale = prefabSlotPlayer.transform.localScale;

        return slotPlayer;
    }

    // private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    // {
    //     LobbyManager.Instance.OnJoinedLobby -= UpdateLobby_Event;
    //     LobbyManager.Instance.OnJoinedLobbyUpdate -= UpdateLobby_Event;
    //     LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;

    //     GameSceneManager.Instance.LoadPreviousScene();
    // }
}