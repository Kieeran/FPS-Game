using System;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : MonoBehaviour
{
    public Button QuitGameButton;
    public Action OnQuitGame;

    void Awake()
    {
        if (!LobbyManager.Instance.IsLobbyHost()) QuitGameButton.gameObject.SetActive(false);

        QuitGameButton.onClick.AddListener(() =>
        {
            OnQuitGame?.Invoke();
        });
    }
}