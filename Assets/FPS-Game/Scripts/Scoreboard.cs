using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public PlayerRoot PlayerRoot { get; private set; }
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    void Awake()
    {
        PlayerRoot = transform.root.GetComponent<PlayerRoot>();
        InGameManager.Instance.OnReceivedPlayerInfo += DisplayPlayerScoreboard;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        InGameManager.Instance.GetAllPlayerInfos(PlayerRoot.NetworkObject.OwnerClientId);
    }

    void DisplayPlayerScoreboard(List<PlayerInfo> playerInfos)
    {
        foreach (var playerInfo in playerInfos)
        {
            GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
            itemGO.SetActive(true);
            if (itemGO.TryGetComponent<PlayerScoreboardItem>(out var item))
            {
                item.Setup(playerInfo.PlayerName, playerInfo.KillCount, playerInfo.DeathCount);
            }
        }
    }

    void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            if (child.gameObject.activeSelf)
                Destroy(child.gameObject);
        }
    }
}