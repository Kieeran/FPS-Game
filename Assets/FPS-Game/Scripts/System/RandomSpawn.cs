using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomSpawn : NetworkBehaviour
{
    [SerializeField] Transform _spawnPositions;
    public List<SpawnPosition> SpawnPositionsList { get; private set; }

    void Awake()
    {
        InitSpawnPositions();
    }
    void InitSpawnPositions()
    {
        SpawnPositionsList = new List<SpawnPosition>();
        foreach (Transform child in _spawnPositions)
        {
            SpawnPositionsList.Add(child.GetComponent<SpawnPosition>());
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