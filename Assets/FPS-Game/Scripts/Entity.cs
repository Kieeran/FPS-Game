using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    private float health;

    public BulletHole prefabBulletHole;

    public Renderer entityRenderer;

    //private BulletHole bulletHole;

    //public float Health
    //{
    //    get
    //    {
    //        return health;
    //    }
    //    set
    //    {
    //        health = value;
    //        Debug.Log(health);

    //        if (health <= 0f)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    public void GetDamage()
    {
        health--;
        if (health <= 0f)
        {
            gameObject.SetActive(false);
            Invoke("SetActiveEntity", 2f);
        }
        if (health == 2)
            entityRenderer.material.SetColor("_Color", new Color(1f, 128f / 255f, 128f / 255f));

        if (health == 1)
            entityRenderer.material.SetColor("_Color", new Color(1f, 0f, 0f));
    }

    public void CreateBulletHole(RaycastHit hit)
    {
        BulletHole bulletHole = BulletHoleManager.Instance.GetBulletHole();
        bulletHole.transform.position = hit.point;
        bulletHole.transform.SetParent(transform);

        Debug.Log("Create bullet hole");
        //Invoke("DestroyBulletHole", 3f);
    }

    //private void DestroyBulletHole()
    //{
    //    Destroy(bulletHole);
    //}

    private void Start()
    {
        health = StartingHealth;
    }

    private void SetActiveEntity()
    {
        gameObject.SetActive(true);
        health = 3f;
        entityRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f));
    }
}
