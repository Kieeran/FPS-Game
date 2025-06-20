using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 2f;        // Tốc độ lên xuống
    public float floatHeight = 0.5f;     // Độ cao lên xuống

    [Header("Rotation Settings")]
    public float rotationSpeed = 45f;    // Độ xoay mỗi giây

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
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
}
