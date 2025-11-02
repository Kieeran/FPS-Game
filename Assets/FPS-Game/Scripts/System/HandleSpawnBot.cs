using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HandleSpawnBot : NetworkBehaviour
{
    [SerializeField] PlayerNetwork botPrefab;
    public Dictionary<PlayerRoot, string> BotLists { get; private set; }

    public override void OnNetworkSpawn()
    {
        BotLists = new();
        SpawnAllBots();
    }

    void SpawnAllBots()
    {
        if (!IsServer) return;
        if (LobbyManager.Instance == null)
        {
            Debug.Log("LobbyManager.Instance == null");
            return;
        }

        int botCount = LobbyManager.Instance.GetBotNum();
        Debug.Log(LobbyManager.Instance.GetBotNum());
        List<string> listID = GenerateUnique4DigitIDs(botCount);
        for (int i = 0; i < botCount; i++)
        {
            SpawnBot(listID[i]);
            Debug.Log($"Spawn bot#{listID[i]}");
        }
    }

    void SpawnBot(string id)
    {
        PlayerNetwork playerNetwork = Instantiate(botPrefab);
        playerNetwork.gameObject.name = "Bot#" + id;
        playerNetwork.GetPlayerRoot().PlayerModel.ChangeRigBuilderState(false);
        Debug.Log($"SpawnBot(): NetworkManager={NetworkManager.Singleton != null}, Parent={playerNetwork.transform.parent}, HasNetworkTransform={playerNetwork.GetComponent<NetworkObject>() != null}");
        playerNetwork.GetComponent<NetworkObject>().Spawn();

        BotLists.Add(playerNetwork.GetPlayerRoot(), id);
    }

    List<string> GenerateUnique4DigitIDs(int count)
    {
        HashSet<string> usedIDs = new();
        System.Random random = new();
        List<string> result = new();

        if (count > 10000)
        {
            Debug.LogError("Không thể tạo quá 10000 ID!");
            return result;
        }

        while (result.Count < count)
        {
            int number = random.Next(0, 10000);
            string id = number.ToString("D4");

            if (usedIDs.Add(id)) // Add trả về true nếu id chưa tồn tại
                result.Add(id);
        }

        return result;
    }
}