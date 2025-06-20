using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 2f;        // Tốc độ lên xuống
    public float floatHeight = 0.5f;     // Độ cao lên xuống

    [Header("Rotation Settings")]
    public float rotationSpeed = 45f;    // Độ xoay mỗi giây

    private Vector3 _startPosition;

    [Header("Raycast Settings")]
    public float raycastDistance;           // Độ dài tối đa raycast
    public float minHeightAboveGround;     // Khoảng cách tối thiểu so với mặt đất
    public LayerMask groundLayer;                 // Layer mặt đất

    public float lifeTime;

    void Start()
    {
        AdjustHeightAboveGround();
        _startPosition = transform.position;

        Invoke(nameof(AutoDestroy), lifeTime);
    }

    void AdjustHeightAboveGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastDistance, groundLayer))
        {
            float currentHeight = hitInfo.distance;

            if (currentHeight < minHeightAboveGround)
            {
                float adjustment = minHeightAboveGround - currentHeight;
                transform.position += Vector3.up * adjustment;
            }
        }
    }

    void Update()
    {
        // Hiệu ứng lên xuống (trục Y)
        float newY = _startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Cập nhật vị trí
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);

        // Xoay quanh trục Y theo chiều kim đồng hồ
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    void AutoDestroy()
    {
        Destroy(gameObject);
    }
}
