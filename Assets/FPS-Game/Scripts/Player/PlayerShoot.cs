using System.Collections;
using Unity.Netcode;
using UnityEngine;

public enum HitArea
{
    Head,
    Torso,
    Leg
}

public enum GunType
{
    None,
    Rifle,
    Sniper,
    Pistol,
}

public class PlayerShoot : PlayerBehaviour
{
    [SerializeField] GameObject _hitEffect;
    Gun _rifle, _sniper, _pistol;

    public override void InitializeAwake()
    {
        base.InitializeAwake();

        _rifle = PlayerRoot.WeaponHolder.Rifle;
        _sniper = PlayerRoot.WeaponHolder.Sniper;
        _pistol = PlayerRoot.WeaponHolder.Pistol;
    }

    float GetDamageByWeaponAndHitArea(GunType gunType, HitArea hitArea)
    {
        return gunType switch
        {
            GunType.Rifle => hitArea switch
            {
                HitArea.Head => _rifle.HeadDamage,
                HitArea.Torso => _rifle.TorsoDamage,
                HitArea.Leg => _rifle.LegDamage,
                _ => 0f
            },

            GunType.Sniper => hitArea switch
            {
                HitArea.Head => _sniper.HeadDamage,
                HitArea.Torso => _sniper.TorsoDamage,
                HitArea.Leg => _sniper.LegDamage,
                _ => 0f
            },

            GunType.Pistol => hitArea switch
            {
                HitArea.Head => _pistol.HeadDamage,
                HitArea.Torso => _pistol.TorsoDamage,
                HitArea.Leg => _pistol.LegDamage,
                _ => 0f
            },

            _ => 0f,
        };
    }

    public void Shoot(float spreadAngle, GunType gunType)
    {
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        HandleServerShoot_ServerRPC(ray.origin, ray.direction, spreadAngle, gunType, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleServerShoot_ServerRPC(
        Vector3 point,
        Vector3 shootDirection,
        float spreadAngle,
        GunType gunType,
        ulong shooterClientId)
    {
        Vector3 spreadDirection = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle), 0) * shootDirection;

        Ray ray = new(point, spreadDirection);

        int layerMask = ~(1 << 2);

        // Raycast bỏ qua Ignore Raycast layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            BulletHitSpawn_ClientRpc(hit.point);

            Transform player = hit.collider.transform.root;

            if (player != null)
            {
                if (player.TryGetComponent<PlayerRoot>(out var playerTargetRoot))
                {
                    if (playerTargetRoot.OwnerClientId == shooterClientId && !playerTargetRoot.IsCharacterBot())
                    {
                        Debug.Log("Self-shoot");
                        return;
                    }

                    HitArea hitArea;
                    if (hit.transform.CompareTag("PlayerHead"))
                        hitArea = HitArea.Head;
                    else if (hit.transform.CompareTag("PlayerTorso"))
                        hitArea = HitArea.Torso;
                    else if (hit.transform.CompareTag("PlayerLeg"))
                        hitArea = HitArea.Leg;
                    else
                    {
                        Debug.Log("Player part unvalid");
                        return;
                    }

                    float damage = GetDamageByWeaponAndHitArea(gunType, hitArea);

                    if (damage > 0)
                    {
                        player.GetComponent<PlayerTakeDamage>().TakeDamage(
                            damage,
                            playerTargetRoot.IsCharacterBot() ?
                                playerTargetRoot.NetworkObjectId : playerTargetRoot.OwnerClientId,
                            shooterClientId,
                            playerTargetRoot.IsCharacterBot()
                        );

                        Debug.Log($"Gun: {gunType}, damage: {damage}");
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