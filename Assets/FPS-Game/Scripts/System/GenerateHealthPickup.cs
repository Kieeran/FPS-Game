using UnityEngine;

public class GenerateHealthPickup : MonoBehaviour
{
    [SerializeField] GameObject _healthPickupPrefab;

    public void DropHealthPickup(Vector3 dropPos)
    {
        GameObject healthPickup = Instantiate(_healthPickupPrefab);
        healthPickup.transform.position = dropPos;
    }
}
