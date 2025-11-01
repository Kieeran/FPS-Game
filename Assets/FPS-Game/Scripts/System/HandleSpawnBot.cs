using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HandleSpawnBot : NetworkBehaviour
{
    [SerializeField] PlayerNetwork botPrefab;

    public override void OnNetworkSpawn()
    {
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
        for (int i = 0; i < botCount; i++)
        {
            SpawnBot();
            Debug.Log("SpawnBot();");
        }
    }

    void SpawnBot()
    {
        PlayerNetwork playerNetwork = Instantiate(botPrefab);
        playerNetwork.gameObject.name = "Bot";
        playerNetwork.GetPlayerRoot().PlayerModel.ChangeRigBuilderState(false);
        Debug.Log($"SpawnBot(): NetworkManager={NetworkManager.Singleton != null}, Parent={playerNetwork.transform.parent}, HasNetworkTransform={playerNetwork.GetComponent<NetworkObject>() != null}");
        playerNetwork.GetComponent<NetworkObject>().Spawn();
    }
}