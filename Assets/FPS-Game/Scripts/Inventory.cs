using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    private bool isReloading;

    private int currentMagazineAmmo;
    private int totalAmmo;

    private float fillAmountOffset;
    private float alphaOffset;
    private float startAlpha;

    public bool GetIsReloading() { return isReloading; }

    [SerializeField] private TMP_Text currentMagazineAmmoText;
    [SerializeField] private TMP_Text totalAmmoText;
    [SerializeField] private Image reloadUI;

    private void Start()
    {
        fillAmountOffset = 0.01f;
        alphaOffset = 0.01f;

        startAlpha = reloadUI.color.a;
        reloadUI.gameObject.SetActive(false);
    }

    public void SetText(int currentMagazineAmmo, int totalAmmo)
    {
        currentMagazineAmmoText.text = currentMagazineAmmo.ToString();
        totalAmmoText.text = totalAmmo.ToString();
    }

    public void StartReloadUI(int currentMagazineAmmo, int totalAmmo)
    {
        this.currentMagazineAmmo = currentMagazineAmmo;
        this.totalAmmo = totalAmmo;

        isReloading = true;
        reloadUI.gameObject.SetActive(true);
    }

    private void Update()
    {
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
                    SetText(currentMagazineAmmo, totalAmmo);

                    //Debug.Log("Reloading done!!!");
                }
            }
        }
    }
}