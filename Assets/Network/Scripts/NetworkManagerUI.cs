using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    // [SerializeField] private GameObject primaryWeapon;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() => {
            // primaryWeapon.SetActive(true);
            NetworkManager.Singleton.StartHost();
        });
        clientBtn.onClick.AddListener(() => {
            // primaryWeapon.SetActive(true);
            NetworkManager.Singleton.StartClient();
        });
    }
}
