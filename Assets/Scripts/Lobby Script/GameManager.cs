using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static int currentIndex;

    void Start () {
       Scene currentScene = SceneManager.GetActiveScene();;
       currentIndex = currentScene.buildIndex;
    }

    void Update () {
        LoadChatUI();
    }

    public void LoadChatUI() {
        Debug.Log(currentIndex);
        if (currentIndex == 2) {
            ChatCanvasUI.Instance.Show();
        }
    }
}