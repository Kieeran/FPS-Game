using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;

public class AuthenticateUI : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            // GameSceneManager.Instance.LoadNextScene();
            GameSceneManager.Instance.LoadScene("Lobby List");

            if (UnityServices.State == ServicesInitializationState.Initialized)
                return;

            LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
        });
    }
}