using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HandleSpawnBot : NetworkBehaviour
{
    public PlayerNetwork BotPrefab;
    public PlayerNetwork BotObj;
    void SpawnBot()
    {
        BotObj = Instantiate(BotPrefab);
        BotObj.gameObject.name = "Bot";
        BotObj.GetPlayerRoot().PlayerModel.ChangeRigBuilderState(false);
        Debug.Log($"SpawnBot(): NetworkManager={NetworkManager.Singleton != null}, Parent={BotObj.transform.parent}, HasNetworkTransform={BotObj.GetComponent<NetworkObject>() != null}");
        BotObj.GetComponent<NetworkObject>().Spawn();
    }

    void Update()
    {
        if (IsOwner == false) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnBot();
        }
    }
}