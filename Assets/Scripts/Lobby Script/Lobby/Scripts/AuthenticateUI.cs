using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthenticateUI : MonoBehaviour
{
    [SerializeField] private Button authenticateButton;

    // public static uint editNameFlag = 0;

    private void Awake()
    {
        authenticateButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            // editNameFlag = 1;
            // SceneManager.LoadScene("Lobby");
            //GameSceneManager.Instance.SetPlayerName(LobbyManager.Instance.GetPlayerName());
            GameSceneManager.Instance.LoadNextScene();

            //Debug.Log("Authenticate button clicked");
        });
    }

    // private void Hide()
    // {
    //     gameObject.SetActive(false);
    // }
}