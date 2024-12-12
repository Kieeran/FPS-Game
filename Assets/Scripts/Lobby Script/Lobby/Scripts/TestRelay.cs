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
using Cinemachine;

public class TestRelay : MonoBehaviour
{
    public static TestRelay Instance { get; private set; }

    private Camera _camera;
    private Camera playerCamera;
    private CinemachineVirtualCamera playerFollowCamera;

    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Awake() {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    private void EnableCamera()
    {
        playerCamera.gameObject.SetActive(true);
        playerFollowCamera.gameObject.SetActive(true);
        //playerUI.gameObject.SetActive(true);
    }

    [Command]
    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Destroy(_camera);
            EnableCamera();

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

            Destroy(_camera);
            EnableCamera();
            
            NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}
