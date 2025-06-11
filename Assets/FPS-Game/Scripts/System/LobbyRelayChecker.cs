using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using System;

public class LobbyRelayChecker : MonoBehaviour
{
    public float checkIntervalSeconds = 2f;
    public Action onAllPlayersConnected;

    private string lobbyId;
    private bool hasFiredEvent = false;

    /// <summary>
    /// Bắt đầu kiểm tra định kỳ.
    /// </summary>
    public async void StartChecking(string targetLobbyId)
    {
        lobbyId = targetLobbyId;
        await PeriodicCheckLoop();
    }

    /// <summary>
    /// Lặp kiểm tra mỗi vài giây.
    /// </summary>
    private async Task PeriodicCheckLoop()
    {
        while (!hasFiredEvent)
        {
            await CheckAllPlayersConnectedToRelay();
            await Task.Delay((int)(checkIntervalSeconds * 1000));
        }
    }

    /// <summary>
    /// Kiểm tra một lần: so sánh số người trong Lobby và số người trong Netcode.
    /// </summary>
    private async Task CheckAllPlayersConnectedToRelay()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
            int lobbyCount = lobby.Players.Count;
            int connectedClientCount = NetworkManager.Singleton.ConnectedClientsList.Count;

            Debug.Log($"Kiểm tra kết nối: {connectedClientCount}/{lobbyCount}");

            if (connectedClientCount == lobbyCount && !hasFiredEvent)
            {
                Debug.Log("Tất cả người chơi đã kết nối vào Relay.");
                hasFiredEvent = true;
                onAllPlayersConnected?.Invoke();
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning($"Không lấy được lobby: {e.Message}");
        }
    }
}
