using UnityEngine;
using Unity.Netcode;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] public Vector3 hostSpawnPosition = new Vector3(66.8f, 2.87f, 17f);  // Host spawn position
    [SerializeField] public Vector3 clientSpawnPosition = new Vector3(66.8f, 2.87f, 17f); // Client spawn position

    void Start()
    {
        // Register the callback for when a client connects
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Determine spawn position based on whether the client is the host or a client
            Vector3 spawnPosition = clientId == NetworkManager.Singleton.LocalClientId ? hostSpawnPosition : clientSpawnPosition;

            // Spawn the player prefab at the specified position
            GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            // Spawn and assign the player object to the client
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    private void OnDestroy()
    {
        // Unregister the callback when this script is destroyed
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}
