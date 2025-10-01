using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using TMPro;

public class SlotPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button kickPlayerButton;
    [SerializeField] private Button openKickPlayerButton;
    [SerializeField] private Canvas slotCanvas;

    public Canvas GetSlotCanvas() { return slotCanvas; }

    private Player player;

    private void Awake()
    {
        kickPlayerButton.onClick.AddListener(KickPlayer);
    }

    private void Start()
    {
        kickPlayerButton.gameObject.SetActive(false);
    }

    public void SetKickPlayerButtonDisable(bool b)
    {
        openKickPlayerButton.gameObject.SetActive(b);
    }

    public void UpdatePlayer(Player player)
    {
        // Gán player trong list Players của lobby vào trong player của slot chứa player
        this.player = player;

        // gán tên
        UpdatePlayerName(player.Data[LobbyManager.KEY_PLAYER_NAME].Value);

        // LobbyManager.PlayerCharacter playerCharacter =
        //     System.Enum.Parse<LobbyManager.PlayerCharacter>(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value);
        // characterImage.sprite = LobbyAssets.Instance.GetSprite(playerCharacter);
    }

    public void UpdatePlayerName(string name)
    {
        playerNameText.text = name;
    }

    private void KickPlayer()
    {
        if (player != null)
        {
            LobbyManager.Instance.KickPlayer(player.Id);
        }
    }
}