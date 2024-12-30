using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private Image escapeUI;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GameObject scoreBoard;

    [SerializeField] private Transform container;
    [SerializeField] private Transform scoreboardSingleTemplate;

    // public Image GetEscapeUI() { return escapeUI; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        quitGameButton.onClick.AddListener(() =>
        {
            if (IsOwner == false) return;

            NetworkManager.Singleton.Shutdown();
            GameSceneManager.Instance.LoadPreviousScene();
        });

        //scoreboardSingleTemplate.gameObject.SetActive(false);

        //StartCoroutine(DelayEachSecond());
    }

    void Update()
    {
        if (IsOwner == false) return;

        if (playerAssetsInputs.escapeUI == true)
        {
            escapeUI.gameObject.SetActive(!escapeUI.gameObject.activeSelf);

            Cursor.lockState = !escapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;

            playerAssetsInputs.escapeUI = false;
        }

        if (playerAssetsInputs.openScoreboard == true)
        {
            scoreBoard.gameObject.SetActive(!scoreBoard.gameObject.activeSelf);

            playerAssetsInputs.openScoreboard = false;
        }
    }
    IEnumerator DelayEachSecond()
    {
        while (true)
        {
            UpdateScoreboard();

            yield return new WaitForSeconds(2f);
        }
    }

    private void UpdateScoreboard()
    {
        // if (container == null) return;

        // foreach (Transform child in container)
        // {
        //     if (child == scoreboardSingleTemplate) continue;

        //     Destroy(child.gameObject);
        // }

        //UpdateScoreboard_ServerRpc();

        // foreach (NetworkClient networkClient in targetPlayers)
        // {
        //     // Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
        //     // lobbySingleTransform.gameObject.SetActive(true);
        //     // LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
        //     // lobbyListSingleUI.UpdateLobby(lobby);
        // }
    }

    // [ServerRpc(RequireOwnership = false)]
    // public void UpdateScoreboard_ServerRpc()
    // {
    //     IReadOnlyList<NetworkClient> targetPlayers = NetworkManager.Singleton.ConnectedClientsList;

    //     foreach (NetworkClient client in targetPlayers)
    //     {
    //         if (client.PlayerObject.TryGetComponent<PlayerNetwork>(out var playerNetwork))
    //         {
    //             Debug.Log(playerNetwork.playerName.Value);
    //             Debug.Log(playerNetwork.killCount.Value);
    //             Debug.Log(playerNetwork.deathCount.Value);
    //         }
    //     }
    // }
}