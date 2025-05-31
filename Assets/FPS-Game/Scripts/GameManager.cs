using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Linq;
using UnityEngine.Rendering;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    public CinemachineVirtualCamera GetCinemachineVirtualCamera() { return cinemachineVirtualCamera; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {

    }
}