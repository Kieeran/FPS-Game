using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        playerName = "PlayerName";
    }

    public string playerName;

    private void Start()
    {
        //playerName = "PlayerName";
    }

    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }
}