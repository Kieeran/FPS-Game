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

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        List<PlayerNetwork.PlayerInfo> playerInfos = PlayerRoot.PlayerNetwork.GetAllPlayerInfos();

        foreach (var playerInfo in playerInfos)
        {
            GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
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
            Destroy(child.gameObject);
        }
    }
}