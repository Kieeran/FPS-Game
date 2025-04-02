using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class PlayerSwitchWeapon : MonoBehaviour
{
    private PlayerAssetsInputs playerAssetsInputs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.hotkey1 == true)
        {
            // currentWeaponID = 0;
            // SetActiveCurrentWeapon(currentWeaponID);
            // currentGun = availableGuns[currentWeaponID];

            // UIManager.Instance.UpdateBulletsHud(currentGun.GetCurrentMagazineAmmo(), currentGun.GetTotalAmmo());
            // UIManager.Instance.SetCurrentWeaponUI(currentWeaponID);

            // playerInputSystem.shortcut1 = false;
        }

        else if (playerAssetsInputs.hotkey2 == true)
        {
            // currentWeaponID = 1;
            // SetActiveCurrentWeapon(currentWeaponID);
            // currentGun = availableGuns[currentWeaponID];

            // UIManager.Instance.UpdateBulletsHud(currentGun.GetCurrentMagazineAmmo(), currentGun.GetTotalAmmo());
            // UIManager.Instance.SetCurrentWeaponUI(currentWeaponID);

            // playerInputSystem.shortcut2 = false;
        }

        else if (playerAssetsInputs.hotkey3 == true)
        {
            // currentWeaponID = 2;
            // SetActiveCurrentWeapon(currentWeaponID);
            // currentGun = availableGuns[currentWeaponID];

            // UIManager.Instance.UpdateBulletsHud(currentGun.GetCurrentMagazineAmmo(), currentGun.GetTotalAmmo());
            // UIManager.Instance.SetCurrentWeaponUI(currentWeaponID);

            // playerInputSystem.shortcut3 = false;
        }
    }
}