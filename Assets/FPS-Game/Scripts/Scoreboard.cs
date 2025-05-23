using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    // public void AddInfoToScoreBoard(List<PlayerNetwork.PlayerInfo> playerInfos)
    // {
    //     foreach (PlayerNetwork.PlayerInfo playerInfo in playerInfos)
    //     {
    //         GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
    //         if (itemGO.TryGetComponent<PlayerScoreboardItem>(out var item))
    //         {
    //             item.Setup(playerInfo.Name + "", playerInfo.KillCount, playerInfo.DeathCount);
    //         }
    //     }
    // }

    void OnEnable()
    {
        // foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        // {
        //     if (networkObject.TryGetComponent<PlayerNetwork>(out var playerNetwork))
        //     {
        //         GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
        //         PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
        //         if (item != null)
        //         {
        //             item.Setup(playerNetwork.playerName, playerNetwork.KillCount.Value, playerNetwork.DeathCount.Value);
        //         }
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