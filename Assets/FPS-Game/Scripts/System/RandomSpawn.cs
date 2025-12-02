using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RandomSpawn : NetworkBehaviour
{
    public List<SpawnPosition> SpawnPositionsList { get; private set; }
    Transform spawnPositions;

    public override void OnNetworkSpawn()
    {
        InitSpawnPositions();
    }

    void InitSpawnPositions()
    {
        SpawnPositionsList = new List<SpawnPosition>();
        spawnPositions = GameObject.FindGameObjectsWithTag("NavigationPoint").FirstOrDefault().GetComponent<SpawnInGameManager>().GetSpawnPositions();
        if (spawnPositions != null)
        {
            foreach (Transform child in spawnPositions)
            {
                SpawnPositionsList.Add(child.GetComponent<SpawnPosition>());
            }
        }
    }

    public SpawnPosition GetRandomPos()
    {
        if (SpawnPositionsList == null || SpawnPositionsList.Count == 0)
        {
            Debug.LogError("SpawnPositionsList is empty!");
            return null;
        }

        return SpawnPositionsList[Random.Range(0, SpawnPositionsList.Count)];
    }
}