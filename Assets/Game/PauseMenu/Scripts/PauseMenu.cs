using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Pressed");
            if (isPaused) 
            {
                ResumeGame();
            } 
            else 
            {
                PauseGame();
            }
        }
    }

    public void PauseGame() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        isPaused = false;
        AuthenticationService.Instance.SignOut();
        NetworkManager.Singleton.Shutdown();
        // If using Relay, ensure to disconnect:
        // await RelayService.Instance.DisconnectAsync();
        SceneManager.LoadScene("SignIn");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
