using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using PlayerAssets;
using UnityEditor;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private List<Image> weaponImages;

    private List<GameObject> playerWeapons;
    private PlayerAssetsInputs playerAssetsInputs;
    private void Start()
    {
        playerWeapons = PlayerManager.Instance.GetPlayerWeapon().GetPlayerWeapons();
        //playerAssetsInputs = PlayerInput.Instance.GetPlayerAssetsInputs();
        SetActiveWeapon(0);
    }

    private void SetActiveWeapon(int WeaponID)
    {
        for (int i = 0; i < playerWeapons.Count; i++)
        {
            if (i == WeaponID)
            {
                playerWeapons[i].gameObject.SetActive(true);
                weaponImages[i].color = new Color(1, 1, 1, 1);
            }

            else
            {
                playerWeapons[i].gameObject.SetActive(false);
                weaponImages[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    private void Update()
    {
        if (playerAssetsInputs.hotkey1 == true)
        {
            SetActiveWeapon(0);
            playerAssetsInputs.hotkey1 = false;
        }

        else if (playerAssetsInputs.hotkey2 == true)
        {
            SetActiveWeapon(1);
            playerAssetsInputs.hotkey2 = false;
        }

        else if (playerAssetsInputs.hotkey3 == true)
        {
            SetActiveWeapon(2);
            playerAssetsInputs.hotkey3 = false;
        }

        else if (playerAssetsInputs.hotkey4 == true)
        {
            SetActiveWeapon(3);
            playerAssetsInputs.hotkey4 = false;
        }

        else if (playerAssetsInputs.hotkey5 == true)
        {
            // SetActiveWeapon(4);
            playerAssetsInputs.hotkey5 = false;
        }
    }

    // void SwitchBulletsHud(GameObject obj)
    // {
    //     Magazine magazine = obj.GetComponent<Magazine>();
    //     if (magazine == null) return;

    //     int currentMagazineAmmo = magazine.GetCurrentMagazineAmmo();
    //     int totalAmmo = weapon1.GetComponent<Magazine>().GetTotalAmmo();

    //     Inventory.Instance.SetText(currentMagazineAmmo, totalAmmo);
    // }
}