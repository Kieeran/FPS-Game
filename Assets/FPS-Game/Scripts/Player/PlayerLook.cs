using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform _targetLook;
    [SerializeField] float distance = 2f;

    void Update()
    {
        UpdateLookTarget();
    }

    void UpdateLookTarget()
    {
        if (_targetLook == null || Camera.main == null)
            return;

        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
        _targetLook.position = targetPosition;
    }
}