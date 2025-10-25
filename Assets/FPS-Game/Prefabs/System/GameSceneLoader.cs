using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public GameObject InGameManagerObj;
    NetworkObject networkObject;
    void Awake()
    {
        networkObject = Instantiate(InGameManagerObj).GetComponent<NetworkObject>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Play Scene")
        {
            if (InGameManager.Instance == null)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    networkObject.Spawn();
                }
            }
        }
    }

}
