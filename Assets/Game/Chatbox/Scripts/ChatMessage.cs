using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI messageText;

    public void SetText(string str) { messageText.text = str; }

}
