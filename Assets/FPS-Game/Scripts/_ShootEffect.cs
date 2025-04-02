using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class _ShootEffect : NetworkBehaviour
{
    [SerializeField] private bool IsRifle;
    [SerializeField] private bool IsPistol;
    public Canvas canvas;

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
        }
    }

    public void ActiveShootEffect()
    {
        if (IsOwner == false) return;

        if (IsRifle)
        {
            StartCoroutine(RifleRecoil());
        }

        else if (IsPistol)
        {
            StartCoroutine(PistolRecoil());
        }
    }

    // void RifleRecoil()
    // {
    //     if (IsOwner == false) return;

    //     transform.localPosition -= Vector3.forward * Time.deltaTime * 10f;
    //     if (enableRecoil == true)
    //     {
    //         if (randomizeRecoil == true)
    //         {
    //             float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
    //             float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

    //             transform.localRotation *= Quaternion.Euler(xRecoil, yRecoil, 1f);
    //         }
    //     }
    // }

    IEnumerator RifleRecoil()
    {
        transform.Find("AK47").GetComponent<Animator>().Play("AK47_Recoil", -1, 0);

        yield return new WaitForSeconds(0.1f);

        transform.Find("AK47").GetComponent<Animator>().Play("AK47_Idle");
    }

    IEnumerator PistolRecoil()
    {
        transform.Find("Glock").GetComponent<Animator>().Play("Glock_Recoil", -1, 0);

        yield return new WaitForSeconds(0.2f);

        transform.Find("Glock").GetComponent<Animator>().Play("Glock_Idle");
    }
}
