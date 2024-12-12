using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;

public class TestRelay : MonoBehaviour
{
    // Start is called before the first frame update
    public static TestRelay Instance { get; private set; }

    private void Start()
    {
        // await UnityServices.InitializeAsync();

        // AuthenticationService.Instance.SignedIn += () => {
        //     Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        // };
        
        // await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Awake() {
        Instance = this;
    }

    [Command]
    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return null;
        }
    }

    [Command]
    public async void JoinRelay(string joinCode) {
        try {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}
