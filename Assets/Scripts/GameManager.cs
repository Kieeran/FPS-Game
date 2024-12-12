using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static int currentIndex;

    public static GameManager Instance;

    public GameObject _camera;
    public GameObject playerCamera;
    public GameObject playerFollowCamera;

    void Start () {
        Scene currentScene = SceneManager.GetActiveScene();;
        currentIndex = currentScene.buildIndex;
    }

    void Awake () {
        if (Instance != null)
            Destroy(Instance);
        else Instance = this;
    }
 
    void Update () {
        Destroy(_camera);
        EnableCamera();
        // // LoadChatUI();
        // if (currentIndex == 2) {
        //     // Destroy(TestRelay._camera);
        //     // if (TestRelay._camera == null) Debug.Log("_camera = null");
        //     // if (TestRelay.playerCamera == null) Debug.Log("playerCamera = null");
        //     // if (TestRelay.playerFollowCamera == null) Debug.Log("playerFollowCamera = null");
        //     Destroy(_camera);
        //     EnableCamera();
        // }
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
}