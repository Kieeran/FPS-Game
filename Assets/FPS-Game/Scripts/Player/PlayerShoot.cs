using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] GameObject _hitEffect;

    public void Shoot(float spreadAngle)
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Shoot_ServerRPC(ray.origin, ray.direction, spreadAngle);
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
        int layerMask = ~(1 << 2);

        // Raycast bỏ qua Ignore Raycast layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            BulletHitSpawn_ClientRpc(hit.point);

            Transform player = hit.collider.transform.root;

            PlayerBody playerBody = hit.collider.GetComponent<PlayerBody>();
            PlayerHead playerHead = hit.collider.GetComponent<PlayerHead>();

            if (player != null)
            {
                if (playerBody != null)
                {
                    if (player.TryGetComponent<NetworkObject>(out var networkObject))
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, networkObject.OwnerClientId, OwnerClientId);
                    }
                }

                if (playerHead != null)
                {
                    if (player.TryGetComponent<NetworkObject>(out var networkObject))
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, networkObject.OwnerClientId, OwnerClientId);
                    }
                }
            }

            // Transform rootPlayerTransform = hit.collider.transform;

            // while (rootPlayerTransform != null && rootPlayerTransform.GetComponent<NetworkObject>() == null)
            // {
            //     if (player != null)
            //     {
            //         if (player.TryGetComponent<NetworkObject>(out var networkObject))
            //         {
            //             player.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, networkObject.OwnerClientId, OwnerClientId);
            //         }
            //     }
            //     rootPlayerTransform = rootPlayerTransform.parent;

            // }

            // if (rootPlayerTransform != null)
            // {
            //     if (player != null)
            //     {
            //         if (player.TryGetComponent<NetworkObject>(out var networkObject))
            //         {
            //             player.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, networkObject.OwnerClientId, OwnerClientId);
            //         }
            //         PlayerBody playerBody = rootPlayerTransform.GetComponentInChildren<PlayerBody>(true);
            //         PlayerHead playerHead = rootPlayerTransform.GetComponentInChildren<PlayerHead>(true);

            //         if (playerBody == null) Debug.Log("playerBody = null");
            //         if (playerHead == null) Debug.Log("playerHead = null");

            //         if (playerBody != null)
            //         {
            //             NetworkObject netObj = rootPlayerTransform.GetComponent<NetworkObject>();
            //             rootPlayerTransform.GetComponent<PlayerTakeDamage>().TakeDamage(0.05f, netObj.OwnerClientId, OwnerClientId);
            //         }

            //         if (playerHead != null)
            //         {
            //             NetworkObject netObj = rootPlayerTransform.GetComponent<NetworkObject>();
            //             rootPlayerTransform.GetComponent<PlayerTakeDamage>().TakeDamage(0.1f, netObj.OwnerClientId, OwnerClientId);
            //         }
            //     }
            // }
        }
    }

    [ClientRpc]
    public void BulletHitSpawn_ClientRpc(Vector3 spawnPos)
    {
        GameObject effect = Instantiate(_hitEffect);
        effect.transform.position = spawnPos;
        StartCoroutine(DestroyEffect(effect));
    }

    IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(1);

        Destroy(effect);
    }
}