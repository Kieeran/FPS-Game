using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public static int currentIndex;

    public static GameManager Instance;

    public GameObject _camera;
    public GameObject playerCamera;
    public GameObject playerFollowCamera;

    [SerializeField] private GameObject scoreboard;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        currentIndex = currentScene.buildIndex;

        // scoreboard.SetActive(false);
    }

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    void Update()
    {
        if (currentIndex == 2)
        {
            Destroy(_camera);
            EnableCamera();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }

        // // LoadChatUI();
    }

    // public void LoadChatUI() {
    //     Debug.Log(currentIndex);
    //     if (currentIndex == 2) {
    //         ChatCanvasUI.Instance.Show();
    //     }
    // }

    public void EnableCamera()
    {
        playerCamera.gameObject.SetActive(true);
        playerFollowCamera.gameObject.SetActive(true);
        //playerUI.gameObject.SetActive(true);
    }

    public static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
}