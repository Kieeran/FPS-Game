using UnityEngine;
using Unity.Netcode;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    void OnEnable()
    {
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
    }

    void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}