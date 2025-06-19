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

        HandleServerShoot_ServerRPC(ray.origin, ray.direction, spreadAngle, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleServerShoot_ServerRPC(Vector3 point, Vector3 shootDirection, float spreadAngle, ulong shooterClientId)
    {
        // Tìm súng hiện tại của người bắn (người gọi hàm Shoot_ServerRPC: shooterClientId)
        Gun currentShooterGun;
        NetworkObject shooterNetworkObj = NetworkManager.Singleton.ConnectedClients[shooterClientId].PlayerObject;

        if (!shooterNetworkObj.TryGetComponent<PlayerRoot>(out var shooterPlayerRoot))
        {
            Debug.LogWarning("Shooter PlayerRoot not found");
            return;
        }

        GameObject shooterWeapon = shooterPlayerRoot.WeaponHolder.GetCurrentWeapon();
        if (shooterWeapon.TryGetComponent<Gun>(out var shooterGun))
        {
            currentShooterGun = shooterGun;
        }
        else
        {
            Debug.Log("Shooter gun not found: " + shooterClientId);
            return;
        }
        //======================================================================================================

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
                if (player.TryGetComponent<NetworkObject>(out var networkObject))
                {
                    float damage = 0f;

                    if (hit.transform.CompareTag("PlayerHead"))
                        damage = currentShooterGun.HeadDamage;
                    else if (hit.transform.CompareTag("PlayerTorso"))
                        damage = currentShooterGun.TorsoDamage;
                    else if (hit.transform.CompareTag("PlayerLeg"))
                        damage = currentShooterGun.LegDamage;

                    if (damage > 0)
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(
                            damage,
                            networkObject.OwnerClientId,
                            shooterClientId
                        );

                        Debug.Log($"Gun: {currentShooterGun.gameObject.name}, damage: {damage}");
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