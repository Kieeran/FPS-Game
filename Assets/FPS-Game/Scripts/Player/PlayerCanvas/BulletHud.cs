using TMPro;
using UnityEngine;

public class BulletHud : MonoBehaviour
{
    public TextMeshProUGUI AmmoInfo;
    public ReloadEffect ReloadEffect;

    public void SetAmmoInfoUI(int currentMagazineAmmo, int totalAmmo)
    {
        AmmoInfo.text = currentMagazineAmmo.ToString() + "/" + totalAmmo.ToString();
    }
}