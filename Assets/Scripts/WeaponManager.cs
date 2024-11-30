using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    [SerializeField] private Explosives grenade;
    [SerializeField] private GameObject explosiveEffect;

    public Explosives GetPrefabGrenade() { return grenade; }
    public GameObject GetExplosiveEffect() { return explosiveEffect; }

    // public BulletHole prefabBulletHole;

    // public BulletHole GetBulletHolePrefab() { return prefabBulletHole; }

    // private Queue<BulletHole> bulletHoles;
    // private int amount = 20;

    // private void Start()
    // {
    //     //InitPooling();
    // }

    // private void InitPooling()
    // {
    //     bulletHoles = new Queue<BulletHole>();

    //     for (int i = 0; i < amount; i++)
    //     {
    //         BulletHole bulletHole = Instantiate(prefabBulletHole);
    //         bulletHole.gameObject.SetActive(false);
    //         bulletHoles.Enqueue(bulletHole);
    //     }
    // }

    // public BulletHole GetBulletHole()
    // {
    //     if (bulletHoles.Count <= 0)
    //     {
    //         CreateMoreBulletHoles();
    //     }

    //     BulletHole bulletHole = bulletHoles.Dequeue();
    //     bulletHole.gameObject.SetActive(true);
    //     bulletHole.StartCounting();
    //     return bulletHole;
    // }

    // public void ReturnBulletHole(BulletHole bulletHole)
    // {
    //     bulletHole.gameObject.SetActive(false);
    //     bulletHoles.Enqueue(bulletHole);
    // }

    // private void CreateMoreBulletHoles()
    // {
    //     for (int i = 0; i < amount; i++)
    //     {
    //         BulletHole bulletHole = Instantiate(prefabBulletHole);
    //         bulletHole.gameObject.SetActive(false);
    //         bulletHoles.Enqueue(bulletHole);
    //     }
    // }
}