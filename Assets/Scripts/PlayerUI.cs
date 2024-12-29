using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private Image escapeUI;

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.escapeUI == true)
        {
            escapeUI.gameObject.SetActive(!escapeUI.gameObject.activeSelf);

            playerAssetsInputs.escapeUI = false;
        }
    }
}