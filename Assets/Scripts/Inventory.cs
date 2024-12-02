using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Inventory : NetworkBehaviour
{
    public static int currentMagazineAmmo;
    private int totalAmmo;
    private int magazineCapacity;
    public static bool isReloading;
    private int flag = 0;

    private float fillAmountOffset = 0.01f;
    private float alphaOffset = 0.01f;

    private float startAlpha;

    private TMP_Text _currentMagazineAmmo;
    private TMP_Text _totalAmmo;
    // [SerializeField] private Image reloadUI;
    private Image reloadUI;

    public bool IsMagazineEmpty() { return currentMagazineAmmo <= 0; }
    public bool IsOutOfAmmo() { return totalAmmo <= 0; }

    public bool IsReloading() { return isReloading; }

    public override void OnGainedOwnership()
    {
        reloadUI = GameObject.Find("Reload").GetComponentInChildren<Image>();
        _currentMagazineAmmo.text = currentMagazineAmmo.ToString();
        _totalAmmo.text = totalAmmo.ToString();
    }

    private void Start()
    {
        reloadUI = GameObject.Find("Reload").GetComponentInChildren<Image>();
        
        magazineCapacity = 30;
        currentMagazineAmmo = 30;
        totalAmmo = 90;

        startAlpha = reloadUI.color.a;

        SetText();
        reloadUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        Debug.Log("(Update) isReloading = " + isReloading);
        if (isReloading == true)
        {
            if (reloadUI.fillAmount < 1f)
                reloadUI.fillAmount += fillAmountOffset;
            else
            {
                if (reloadUI.color.a > 0f)
                    reloadUI.color = new Color(1f, 1f, 1f, reloadUI.color.a - alphaOffset);
                else
                {
                    reloadUI.gameObject.SetActive(false);
                    reloadUI.color = new Color(1f, 1f, 1f, startAlpha);
                    reloadUI.fillAmount = 0;

                    isReloading = false;
                    SetText();

                    //Debug.Log("Reloading done!!!");
                }
            }
        }
    }

    public void UpdateBulletsHud()
    {
        currentMagazineAmmo--;
        SetText();
    }

    private void SetText()
    {
        Debug.Log("(SetText) isReloading = " + isReloading);
        magazineCapacity = 30;
        
        
        _currentMagazineAmmo = GameObject.Find("CurrentMagazineAmmo").GetComponentInChildren<TextMeshProUGUI>();
        _totalAmmo = GameObject.Find("TotalAmmo").GetComponentInChildren<TextMeshProUGUI>();
        if (flag == 0)
        {
            reloadUI = GameObject.Find("Reload").GetComponentInChildren<Image>();
            totalAmmo = 90;
            flag = 1; 
        }
        
        _currentMagazineAmmo.text = currentMagazineAmmo.ToString();
        _totalAmmo.text = totalAmmo.ToString();
    }

    private void StartReloadUI()
    {
        isReloading = true;
        reloadUI.gameObject.SetActive(true);
    }

    public void Reload()
    {
        Debug.Log("Reload");
        Debug.Log(currentMagazineAmmo + " - " + magazineCapacity + " - " + totalAmmo);
        if (IsOutOfAmmo())
        {
            Debug.Log("Out of ammo");
            return;
        }

        StartReloadUI();

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