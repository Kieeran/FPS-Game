using UnityEngine;
using Unity.Netcode;

public class _PlayerColorChanger : NetworkBehaviour
{
    public Renderer playerRenderer; // Reference to the Renderer of your player prefab

    // A NetworkVariable to store the color
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);  // Only server can write

    public override void OnNetworkSpawn()
    {
        // Apply the color to the renderer whenever it's changed on the network
        playerColor.OnValueChanged += (Color oldColor, Color newColor) =>
        {
            ApplyColor(newColor);
        };

        // Apply the initial color
        ApplyColor(playerColor.Value);

        // If the local player owns this object, request a color update
        if (IsOwner)
        {
            if (IsHost)
            {
                // Host player, set color to white
                UpdateColorServerRpc(Color.white);
            }
            else
            {
                // Client player, request a random color from the server
                UpdateColorServerRpc(GetRandomColor());
            }
        }
    }

    void ApplyColor(Color color)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = color;  // Apply the color to the renderer
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateColorServerRpc(Color color)
    {
        // The server updates the NetworkVariable for all clients
        playerColor.Value = color;
    }

    Color GetRandomColor()
    {
        // Generate a random color that isn't white
        Color randomColor;
        do
        {
            randomColor = new Color(Random.value, Random.value, Random.value);
        } while (randomColor == Color.white);  // Ensure it's not white
        return randomColor;
    }
}
