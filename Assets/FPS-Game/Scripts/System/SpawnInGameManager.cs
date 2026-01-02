using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnInGameManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameManagerPrefab;
    [SerializeField] Transform spawnPositions;
    [SerializeField] Transform waypoints;
    [SerializeField] Transform areasContainer;
    [SerializeField] TacticalPoints tacticalPoints;

    public Transform GetSpawnPositions() { return spawnPositions; }
    public Transform GetWaypoints() { return waypoints; }
    public Transform GetAreasContainer() { return areasContainer; }
    public List<Transform> GetTacticalPointsList() { return tacticalPoints.TPoints; }

    void Awake()
    {
        // Nếu NetworkManager đã có sẵn và đang lắng nghe thì gọi ngay
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            TrySpawn();
        }
        else
        {
            // Nếu chưa, gắn event để gọi ngay khi server khởi động
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += TrySpawn;
            }
            else
            {
                Debug.LogWarning("[SpawnInGameManager] NetworkManager.Singleton is null; cannot subscribe to OnServerStarted.");
            }
        }
    }

    private void TrySpawn()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (InGameManager.Instance != null)
        {
            Debug.Log("[SpawnInGameManager] Instance đã tồn tại, bỏ qua spawn.");
            return;
        }

        if (inGameManagerPrefab == null)
        {
            Debug.LogError("[SpawnInGameManager] Prefab chưa gán!");
            return;
        }

        var obj = Instantiate(inGameManagerPrefab);
        var netObj = obj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("[SpawnInGameManager] Prefab thiếu NetworkObject!");
            Destroy(obj);
            return;
        }

        netObj.Spawn();
        Debug.Log("[SpawnInGameManager] Spawned InGameManager sớm bằng OnServerStarted.");
    }
}