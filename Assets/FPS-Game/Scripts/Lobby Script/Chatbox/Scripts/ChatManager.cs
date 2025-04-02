using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;
    public static bool isChatting;

    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    // [SerializeField] GameObject chatCanvas;

    public string playerName;

    void Awake() { ChatManager.Singleton = this; }

    // Start is called before the first frame update
    void Start()
    {
        isChatting = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.BackQuote)) {
        //     isChatting = true;
        //     Cursor.visible = true;
        //     Cursor.lockState = CursorLockMode.None;
        //     if (Input.GetKeyDown(KeyCode.Return)) {
        //         SendChatMessage(chatInput.text, EditPlayerName.Instance.GetPlayerName());
        //         chatInput.text = "";
        //         isChatting = false;
        //         Cursor.visible = false;
        //         Cursor.lockState = CursorLockMode.Locked;
        //     }
        // }
        // if (Input.GetKeyDown(KeyCode.Return)) {
        //     SendChatMessage(chatInput.text, EditPlayerName.Instance.GetPlayerName());
        //     chatInput.text = "";
        //     isChatting = false;
        //     Cursor.visible = false;
        //     Cursor.lockState = CursorLockMode.Locked;
        // }
    }

    public void SendChatMessage(string _message, string _fromWho = null) {
        if (string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " > " + _message;
        SendChatMessageServerRpc(S);
    }

    void AddMessage(string msg) {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    [ServerRpc (RequireOwnership = false)]
    void SendChatMessageServerRpc(string message) {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message) {
        ChatManager.Singleton.AddMessage(message);
    }

}