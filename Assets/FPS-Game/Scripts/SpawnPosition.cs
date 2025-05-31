using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    public Vector3 SpawnPos { get; private set; }
    public Quaternion SpawnRot { get; private set; }

    void Awake()
    {
        SpawnPos = transform.position;
        SpawnRot = Quaternion.LookRotation(transform.forward);
    }
}