using System.Collections;
using System.Collections.Generic;
using PlayerAssets;
using UnityEngine;

public class Explosives : Weapon
{
    [SerializeField] private Rigidbody grenadeRB;

    private PlayerAssetsInputs playerAssetsInputs;

    // Start is called before the first frame update
    void Start()
    {
        //playerAssetsInputs = PlayerInput.Instance.GetPlayerAssetsInputs();
    }

    public Rigidbody GetRigibody() { return grenadeRB; }

    // Update is called once per frame
    void Update()
    {
        if (playerAssetsInputs.shoot == true)
        {
            Explosives grenade = Instantiate(WeaponManager.Instance.GetPrefabGrenade());
            grenade.gameObject.transform.position = transform.position + new Vector3(0, 1f, 0);

            Rigidbody rb = grenade.GetRigibody();
            Transform player = Player.Instance.transform;
            float force = 20f;

            rb.useGravity = true;
            rb.AddForce(gameObject.transform.forward * force, ForceMode.Impulse);

            playerAssetsInputs.shoot = false;

            StartCoroutine(DestroyGrenade(grenade.gameObject));

            //gameObject.SetActive(false);
        }
    }

    IEnumerator DestroyGrenade(GameObject grenade)
    {
        yield return new WaitForSeconds(2);

        if (grenade != null)
            Destroy(grenade);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            GameObject explosiveEffect = Instantiate(WeaponManager.Instance.GetExplosiveEffect());
            explosiveEffect.transform.position = gameObject.transform.position;
            StartCoroutine(DestroyExplosiveEffect(explosiveEffect));

            Destroy(gameObject);
        }

    }

    IEnumerator DestroyExplosiveEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3);

        Destroy(effect);
    }
}