using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthenticateUI : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            GameSceneManager.Instance.LoadNextScene();
        });
    }
}