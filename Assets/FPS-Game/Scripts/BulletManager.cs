using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    [SerializeField]
    private Bullet prefabBullet;
    private Queue<Bullet> bullets;

    private int poolingAmount;

    private void Start()
    {
        poolingAmount = 5;
        InitPooling();
    }

    public void InitPooling()
    {
        if (bullets == null) bullets = new Queue<Bullet>();

        for (int i = 0; i < poolingAmount; i++)
        {
            Bullet bullet = Instantiate(prefabBullet);
            bullet.transform.parent = transform;
            bullet.gameObject.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }

    public Bullet GetBullet()
    {
        if (bullets.Count <= 0)
        {
            InitPooling();
        }
        Bullet bullet = bullets.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        //bullet.transform.parent = transform;
        bullet.gameObject.SetActive(false);
        bullets.Enqueue(bullet);
    }

    private void Update()
    {

    }
}