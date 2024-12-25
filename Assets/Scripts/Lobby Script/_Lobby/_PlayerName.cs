using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class _PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshPro playerNameText;

    // Use NetworkVariable to store the player's name
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // Apply the player name whenever it changes on the network
        playerName.OnValueChanged += (FixedString32Bytes oldName, FixedString32Bytes newName) =>
        {
            playerNameText.text = newName.ToString();
        };

        // If this instance is owned by the local player, set the name from the local data
        if (IsOwner)
        {
            string name = EditPlayerName.Instance.GetPlayerName();
            UpdatePlayerNameServerRpc(name);
        }

        // Apply the initial name
        playerNameText.text = playerName.Value.ToString();
    }

    // Use a ServerRpc to update the name
    [ServerRpc (RequireOwnership = false)]
    void UpdatePlayerNameServerRpc(string name)
    {
        playerName.Value = name; // Set the name on the server, which will propagate it to all clients
    }
}
