using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class _ShootEffect : NetworkBehaviour
{
    [SerializeField] private bool IsRifle;
    [SerializeField] private bool IsPistol;

    // Muzzle flash
    [SerializeField] private Image muzzleFlash;
    public Canvas canvas;
    public float muzzleFlashCD;

    // Weapon recoil
    [SerializeField] private bool enableRecoil;
    [SerializeField] private bool randomizeRecoil;
    [SerializeField] private Vector2 randomRecoilConstraints;
    //[SerializeField] private Vector2[] RecoilPattern;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            canvas.gameObject.SetActive(true);
            muzzleFlash.gameObject.SetActive(false);
        }
    }

    public void ActiveShootEffect()
    {
        if (IsOwner == false) return;

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
        if (IsOwner == false) return;

        transform.localPosition -= Vector3.forward * Time.deltaTime * 10f;
        if (enableRecoil == true)
        {
            if (randomizeRecoil == true)
            {
                float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
                float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

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
