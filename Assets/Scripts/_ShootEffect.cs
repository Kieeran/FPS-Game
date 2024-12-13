using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _ShootEffect : MonoBehaviour
{
    [SerializeField] private bool IsRifle;
    [SerializeField] private bool IsPistol;

    // Muzzle flash
    [SerializeField] private Image muzzleFlash;
    public float muzzleFlashCD;

    // Weapon recoil
    [SerializeField] private bool enableRecoil;
    [SerializeField] private bool randomizeRecoil;
    [SerializeField] private Vector2 randomRecoilConStraints;
    //[SerializeField] private Vector2[] RecoilPattern;

    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
    }

    public void ActiveShootEffect()
    {
        if (IsRifle)
        {
            RifleRecoil();
        }

        else if (IsPistol)
        {
            StartCoroutine(PistolRecoil());
        }

        StartCoroutine(MuzzleFlash());
    }
    void RifleRecoil()
    {
        transform.localPosition -= Vector3.forward * Time.deltaTime * 10f;
        if (enableRecoil == true)
        {
            if (randomizeRecoil == true)
            {
                float xRecoil = Random.Range(-randomRecoilConStraints.x, randomRecoilConStraints.x);
                float yRecoil = Random.Range(-randomRecoilConStraints.y, randomRecoilConStraints.y);

                transform.localRotation *= Quaternion.Euler(xRecoil, yRecoil, 1f);
            }
        }
    }

    IEnumerator PistolRecoil()
    {
        //GetComponentInChildren<Animator>().Play("Pistol_Recoil");
        transform.Find("Glock").GetComponent<Animator>().Play("Pistol_Recoil");
        yield return new WaitForSeconds(0.2f);
        //GetComponentInChildren<Animator>().Play("DefaultState");
        transform.Find("Glock").GetComponent<Animator>().Play("DefaultState");
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashCD);
        muzzleFlash.gameObject.SetActive(false);
    }
}
