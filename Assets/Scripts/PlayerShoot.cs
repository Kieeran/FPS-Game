using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using PlayerAssets;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject BulletSpawnPoint;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private PlayerAssetsInputs playerAssetsInputs;
    [SerializeField] private float FireCoolDown;
    private float CurrentCoolDown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            if (CurrentCoolDown <= 0)
            {
                CurrentCoolDown = FireCoolDown;
                Shoot();
            }
        }
        CurrentCoolDown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Debug.Log(hit.point);

            GameObject effect = Instantiate(hitEffect);
            effect.transform.position = hit.point;

            StartCoroutine(DestroyEffect(effect));

            PlayerBody playerBody = hit.collider.GetComponent<PlayerBody>();
            PlayerHead playerHead = hit.collider.GetComponent<PlayerHead>();


            if (playerBody != null)
            {
                // if (IsOwner == true)
                // {
                //     playerBody.Hit();
                // }

                // Transform player = hit.collider.transform.parent;

                // if (player != null)
                // {
                //     NetworkObject networkObject = player.GetComponent<NetworkObject>();

                //     if (networkObject != null)
                //     {
                //         player.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, networkObject.OwnerClientId);
                //     }
                // }
            }

            if (playerHead != null)
            {
                // if (IsOwner == true)
                // {
                //     playerHead.Hit();
                // }

                // Transform player = hit.collider.transform.parent.parent;

                // if (player != null)
                // {
                //     NetworkObject networkObject = player.GetComponent<NetworkObject>();

                //     if (networkObject != null)
                //     {
                //         player.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, networkObject.OwnerClientId);
                //     }
                // }
            }
        }
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(1);

        Destroy(effect);
    }
}