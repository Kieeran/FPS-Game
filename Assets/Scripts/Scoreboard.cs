using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;

    [SerializeField] Transform playerScoreboardList;

    void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();
        // Player player0 = players[0];

        foreach (Player player in players)
        {
            GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if (item != null)
            {
                item.Setup(player.Data[LobbyManager.KEY_PLAYER_NAME].Value);
            }

            Debug.Log("Player's name: " + player.Data[LobbyManager.KEY_PLAYER_NAME].Value + " | Player's ID: " + player.Id);
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