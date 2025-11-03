using System;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : PlayerBehaviour
{
    public Button QuitGameButton;

    void Awake()
    {
        if (!LobbyManager.Instance.IsLobbyHost()) QuitGameButton.gameObject.SetActive(false);

        QuitGameButton.onClick.AddListener(() =>
        {
            PlayerRoot.Events.InvokeOnQuitGame();
        });
    }
}