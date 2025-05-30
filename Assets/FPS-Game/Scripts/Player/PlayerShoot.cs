using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] GameObject _hitEffect;
    [SerializeField] float _headDamage;
    [SerializeField] float _torsoDamage;
    [SerializeField] float _legDamage;

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

            if (player != null)
            {
                if (hit.transform.CompareTag("PlayerHead"))
                {
                    if (player.TryGetComponent<NetworkObject>(out var networkObject))
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(_headDamage, "Headshot", networkObject.OwnerClientId, OwnerClientId);
                    }
                }

                else if (hit.transform.CompareTag("PlayerTorso"))
                {
                    if (player.TryGetComponent<NetworkObject>(out var networkObject))
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(_torsoDamage, "Torsoshot", networkObject.OwnerClientId, OwnerClientId);
                    }
                }

                else if (hit.transform.CompareTag("PlayerLeg"))
                {
                    if (player.TryGetComponent<NetworkObject>(out var networkObject))
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(_legDamage, "Legshot", networkObject.OwnerClientId, OwnerClientId);
                    }
                }
            }
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