using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class AuthenticateUI : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameSceneManager.Instance.LoadNextScene();

            if (UnityServices.State == ServicesInitializationState.Initialized)
                return;

            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
        });
    }
}