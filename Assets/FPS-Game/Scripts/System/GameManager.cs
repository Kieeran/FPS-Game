using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] Transform _spawnPositions;

    public static GameManager Instance { get; private set; }
    public CinemachineVirtualCamera GetCinemachineVirtualCamera() { return _cinemachineVirtualCamera; }
    public List<Transform> SpawnPositionsList { get; private set; }
    public TimePhaseCounter TimePhaseCounter { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitSpawnPositions();

        TimePhaseCounter = GetComponent<TimePhaseCounter>();
    }

    void InitSpawnPositions()
    {
        SpawnPositionsList = new List<Transform>();
        foreach (Transform child in _spawnPositions)
        {
            SpawnPositionsList.Add(child);
        }
    }

    public Transform GetRandomPos()
    {
        if (SpawnPositionsList == null || SpawnPositionsList.Count == 0)
        {
            Debug.LogError("SpawnPositionsList is empty!");
            return null;
        }

        return SpawnPositionsList[Random.Range(0, SpawnPositionsList.Count)];
    }

    public void RemoveBlockingWalls()
    {

    }
}