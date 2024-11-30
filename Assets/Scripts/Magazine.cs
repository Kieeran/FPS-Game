using UnityEngine;

public class Magazine : MonoBehaviour
{
    private int currentMagazineAmmo;
    [SerializeField] private int totalAmmo;
    [SerializeField] private int magazineCapacity;

    public bool IsMagazineEmpty() { return currentMagazineAmmo <= 0; }
    public bool IsOutOfAmmo() { return totalAmmo <= 0; }

    public int GetCurrentMagazineAmmo() { return currentMagazineAmmo; }
    public int GetTotalAmmo() { return totalAmmo; }

    public bool IsReloading() { return Inventory.Instance.GetIsReloading(); }

    private void Start()
    {
        currentMagazineAmmo = magazineCapacity;
        Inventory.Instance.SetText(currentMagazineAmmo, totalAmmo);
    }

    public void UpdateBulletsHud()
    {
        currentMagazineAmmo--;
        Inventory.Instance.SetText(currentMagazineAmmo, totalAmmo);
    }

    public void Reload()
    {
        if (IsOutOfAmmo())
        {
            Debug.Log("Out of ammo");
            return;
        }

        Inventory.Instance.StartReloadUI(currentMagazineAmmo, totalAmmo);

        if (IsMagazineEmpty())
        {
            if (totalAmmo >= magazineCapacity)
            {
                currentMagazineAmmo = magazineCapacity;
                totalAmmo -= magazineCapacity;
            }

            else
            {
                currentMagazineAmmo = totalAmmo;
                totalAmmo = 0;
            }
        }

        else
        {
            if (totalAmmo >= magazineCapacity - currentMagazineAmmo)
            {
                totalAmmo -= magazineCapacity - currentMagazineAmmo;
                currentMagazineAmmo = magazineCapacity;
            }

            else
            {
                currentMagazineAmmo += totalAmmo;
                totalAmmo = 0;
            }
        }

        //Debug.Log("Reloading...");
    }
}