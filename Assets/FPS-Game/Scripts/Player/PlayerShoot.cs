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

    public void Shoot(float spreadAngle)
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Shoot_ServerRPC(ray.origin, ray.direction, spreadAngle);

        // if (Physics.Raycast(ray, out RaycastHit hit))
        // {
        //     // Debug.Log(hit.point);

        //     // GameObject effect = Instantiate(hitEffect);
        //     // effect.transform.position = hit.point;

        //     // StartCoroutine(DestroyEffect(effect));

        //     BulletHitSpawnServerRpc(hit.point);

        //     PlayerBody playerBody = hit.collider.GetComponent<PlayerBody>();
        //     PlayerHead playerHead = hit.collider.GetComponent<PlayerHead>();

        //     if (playerBody != null)
        //     {
        //         // if (IsOwner == true)
        //         // {
        //         //     playerBody.Hit();
        //         // }

        //         Transform player = hit.collider.transform.parent;

        //         if (player != null)
        //         {
        //             NetworkObject networkObject = player.GetComponent<NetworkObject>();

        //             if (networkObject != null)
        //             {
        //                 //Debug.Log($"Detech {networkObject.OwnerClientId} body");
        //                 player.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, networkObject.OwnerClientId);
        //             }
        //         }
        //     }

        //     if (playerHead != null)
        //     {
        //         // if (IsOwner == true)
        //         // {
        //         //     playerHead.Hit();
        //         // }

        //         Transform player = hit.collider.transform.parent.parent;

        //         if (player != null)
        //         {
        //             NetworkObject networkObject = player.GetComponent<NetworkObject>();

        //             if (networkObject != null)
        //             {
        //                 //Debug.Log($"Detech {networkObject.OwnerClientId} head");
        //                 player.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, networkObject.OwnerClientId);
        //             }
        //         }
        //     }
        // }
    }

    [ServerRpc(RequireOwnership = false)]
    public void Shoot_ServerRPC(Vector3 point, Vector3 shootDirection, float spreadAngle)
    {
        Vector3 spreadDirection = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0
        ) * shootDirection;

        Ray ray = new(point, spreadDirection);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Debug.Log(hit.point);

            // GameObject effect = Instantiate(hitEffect);
            // effect.transform.position = hit.point;

            // StartCoroutine(DestroyEffect(effect));

            // BulletHitSpawnServerRpc(hit.point);

            BulletHitSpawn_ClientRpc(hit.point);

            // PlayerBody playerBody = hit.collider.GetComponentInChildren<PlayerBody>();
            // PlayerHead playerHead = hit.collider.GetComponentInChildren<PlayerHead>();

            // if (playerBody != null)
            // {
            //     Transform player = hit.collider.transform.parent;

            //     if (player != null)
            //     {
            //         NetworkObject networkObject = player.GetComponent<NetworkObject>();

            //         if (networkObject != null)
            //         {
            //             player.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, networkObject.OwnerClientId, OwnerClientId);
            //         }
            //     }
            // }

            // if (playerHead != null)
            // {
            //     Transform player = hit.collider.transform.parent.parent;

            //     if (player != null)
            //     {
            //         NetworkObject networkObject = player.GetComponent<NetworkObject>();

            //         if (networkObject != null)
            //         {
            //             player.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, networkObject.OwnerClientId, OwnerClientId);
            //         }
            //     }
            // }

            Transform rootPlayerTransform = hit.collider.transform;

            while (rootPlayerTransform != null && rootPlayerTransform.GetComponent<NetworkObject>() == null)
            {
                rootPlayerTransform = rootPlayerTransform.parent;
            }

            if (rootPlayerTransform != null)
            {
                PlayerBody playerBody = rootPlayerTransform.GetComponentInChildren<PlayerBody>(true);
                PlayerHead playerHead = rootPlayerTransform.GetComponentInChildren<PlayerHead>(true);

                if (playerBody == null) Debug.Log("playerBody = null");
                if (playerHead == null) Debug.Log("playerHead = null");

                if (playerBody != null)
                {
                    NetworkObject netObj = rootPlayerTransform.GetComponent<NetworkObject>();
                    rootPlayerTransform.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, netObj.OwnerClientId, OwnerClientId);
                }

                if (playerHead != null)
                {
                    NetworkObject netObj = rootPlayerTransform.GetComponent<NetworkObject>();
                    rootPlayerTransform.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, netObj.OwnerClientId, OwnerClientId);
                }
            }
        }
    }

    // [ServerRpc(RequireOwnership = false)]
    // public void BulletHitSpawnServerRpc(Vector3 spawnPos)
    // {
    //     BulletHitSpawnClientRpc(spawnPos);
    // }

    [ClientRpc]
    public void BulletHitSpawn_ClientRpc(Vector3 spawnPos)
    {
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = spawnPos;
        StartCoroutine(DestroyEffect(effect));
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(1);

        Destroy(effect);
    }
}