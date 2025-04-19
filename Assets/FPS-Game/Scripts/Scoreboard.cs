using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Unity.Netcode;
using Mono.CSharp;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    // public List<NetworkObject> playerList = new();

    void OnEnable()
    {
        // if (IsClient)
        // {
        //     Debug.Log("Player is Client");
        // }

        // if (!IsClient)
        // {
        //     Debug.Log("Player is not Client");
        //     if (IsHost)
        //     {
        //         Debug.Log("Player is Host");
        //     }
        // }
        // if (IsClient)
        // {
        //     // Scoreboard_ServerRpc();
        //     Debug.Log("Client Scoreboard!");
        //     foreach (NetworkObject player in playerList)
        //     {
        //         if (player.TryGetComponent<PlayerNetwork>(out var playerNetwork))
        //         {
        //             GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
        //             PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
        //             if (item != null)
        //             {
        //                 item.Setup(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
        //             }
        //             // Scoreboard_ClientRpc(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
        //         }
        //     }
        // }

        // if (!IsClient)
        // {
        // Scoreboard_ClientRpc("a", 69, 96);
        // Scoreboard_ServerRpc();
        // Debug.Log("Host Scoreboard!");
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            if (networkObject.TryGetComponent<PlayerNetwork>(out var playerNetwork))
            {
                GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
                PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
                if (item != null)
                {
                    item.Setup(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
                }
            }
        }
        // }
    }

    // [ServerRpc(RequireOwnership = false)]
    // public void Scoreboard_ServerRpc()
    // {
    //     Debug.Log("In Scoreboard_ServerRpc");
    //     foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
    //     {
    //         NetworkObject player = client.PlayerObject;

    //         if (player.TryGetComponent<PlayerNetwork>(out var playerNetwork))
    //         {
    //             // GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
    //             // PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
    //             // if (item != null)
    //             // {
    //             //     item.Setup(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
    //             // }
    //             Scoreboard_ClientRpc(playerNetwork.playerName, playerNetwork.killCount.Value, playerNetwork.deathCount.Value);
    //         }
    //     }
    // }

    // [ClientRpc]
    // public void Scoreboard_ClientRpc(string playerName, int killCount, int deathCount)
    // {
    //     Debug.Log("In Scoreboard_ClientRpc");
    //     GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
    //     PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
    //     if (item != null)
    //     {
    //         item.Setup(playerName, killCount, deathCount);
    //     }
    // }

    void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }

}