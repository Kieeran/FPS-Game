using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    void OnEnable()
    {
        // Player[] players = GameManager.GetAllPlayers();

        // foreach (Player player in players)
        // {
        //     GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
        //     PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
        //     if (item != null)
        //     {
        //         item.Setup(player.Data[LobbyManager.KEY_PLAYER_NAME].Value);
        //     }
        // }
    }

    void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }

}