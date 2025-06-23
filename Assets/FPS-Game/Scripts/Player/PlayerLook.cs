using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform TargetLook;
    [SerializeField] float distance = 2f;

    void Update()
    {
        UpdateTargetLook();
    }

    void UpdateTargetLook()
    {
        if (TargetLook == null || Camera.main == null)
            return;

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
        TargetLook.position = targetPosition;
    }
}