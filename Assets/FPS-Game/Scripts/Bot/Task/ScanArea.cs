using AIBot;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class ScanArea : Action
    {
        [Header("Scan Settings")]
        [UnityEngine.Tooltip("Tốc độ quay ngang (độ/giây)")]
        [SerializeField] float yawSpeed = 40f;

        [Tooltip("Tốc độ quay dọc (độ/giây)")]
        [SerializeField] float pitchSpeed = 30f;

        [Tooltip("Số lượng điểm dừng khi quét 180° (3-5 điểm)")]
        [SerializeField] int scanStopPoints = 4;

        [Tooltip("Thời gian dừng tại mỗi điểm để quan sát (giây)")]
        [SerializeField] float pauseDuration = 0.6f;

        [Header("Pitch Range")]
        [Tooltip("Góc nhìn lên tối đa")]
        [SerializeField] float topPitch = -25f;

        [Tooltip("Góc nhìn xuống tối đa")]
        [SerializeField] float bottomPitch = 35f;

        [Tooltip("Có quét cả lên/xuống tại mỗi điểm dừng không")]
        [SerializeField] bool scanVertically = true;

        [Header("References")]
        [Tooltip("Vector3 lưu góc Euler của bot (x=pitch, y=yaw)")]
        [SerializeField] SharedVector3 lookEuler;

        [Tooltip("Data về vị trí di chuyển cuối của player)")]
        [SerializeField] SharedTPointData data;

        [Header("Scan Pattern")]
        [Tooltip("Bắt đầu quét từ bên trái hay bên phải")]
        [SerializeField] bool startFromLeft = true;

        [Header("Debug")]
        [SerializeField] bool showDebugLogs = true;

        // State machine
        enum ScanState
        {
            MovingToStartPosition,
            RotatingToNext,
            PausingAtPoint,
            ScanningVertical,
            ReturningToCenter,
            Completed
        }

        ScanState currentState;
        int currentStopPoint = 0;

        float centerYaw = 0f;        // Góc trung tâm (hướng LKP)
        float startScanYaw = 0f;     // Góc bắt đầu quét (-90° hoặc +90° từ center)
        float endScanYaw = 0f;       // Góc kết thúc quét
        float targetYaw = 0f;        // Góc đích hiện tại

        // Vertical scan
        bool isGoingUp = true;
        float targetPitch = 0f;

        // Timer
        float pauseTimer = 0f;

        public override void OnStart()
        {
            base.OnStart();

            if (!data.Value.IsValid()) return;

            // Lấy hướng LKP
            centerYaw = data.Value.Rotation.eulerAngles.y;

            // Tính góc bắt đầu và kết thúc (quét 180°)
            if (startFromLeft)
            {
                startScanYaw = NormalizeAngle(centerYaw - 90f);  // Bắt đầu từ trái
                endScanYaw = NormalizeAngle(centerYaw + 90f);    // Kết thúc ở phải
            }
            else
            {
                startScanYaw = NormalizeAngle(centerYaw + 90f);  // Bắt đầu từ phải
                endScanYaw = NormalizeAngle(centerYaw - 90f);    // Kết thúc ở trái
            }

            currentStopPoint = 0;
            pauseTimer = 0f;
            isGoingUp = true;

            // Reset pitch và đặt mục tiêu đầu tiên
            SetRotation(0f, lookEuler.Value.y);
            targetYaw = startScanYaw;
            currentState = ScanState.MovingToStartPosition;

            if (showDebugLogs)
            {
                Debug.Log($"[ScanAreaAtLKP] Bắt đầu quét 180°");
                Debug.Log($"  LKP Yaw: {centerYaw:F1}°, Start: {startScanYaw:F1}°, End: {endScanYaw:F1}°");
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (!data.Value.IsValid()) return TaskStatus.Failure;

            switch (currentState)
            {
                case ScanState.MovingToStartPosition:
                    HandleMovingToStart();
                    break;

                case ScanState.RotatingToNext:
                    HandleRotating();
                    break;

                case ScanState.PausingAtPoint:
                    HandlePausing();
                    break;

                case ScanState.ScanningVertical:
                    HandleVerticalScan();
                    break;

                case ScanState.ReturningToCenter:
                    HandleReturning();
                    break;

                case ScanState.Completed:
                    return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        void HandleMovingToStart()
        {
            float currentYaw = lookEuler.Value.y;
            float newYaw = MoveTowardsAngle(currentYaw, targetYaw, yawSpeed * Time.deltaTime);

            SetRotation(0f, newYaw);

            // Đã đến vị trí bắt đầu
            if (Mathf.Abs(Mathf.DeltaAngle(newYaw, targetYaw)) < 1f)
            {
                SetRotation(0f, targetYaw);
                currentStopPoint = 0;

                if (showDebugLogs)
                    Debug.Log($"[ScanAreaAtLKP] Bắt đầu quét từ điểm 0");

                // Bắt đầu pause hoặc vertical scan tại điểm đầu tiên
                if (scanVertically)
                {
                    isGoingUp = true;
                    targetPitch = topPitch;
                    currentState = ScanState.ScanningVertical;
                }
                else
                {
                    pauseTimer = 0f;
                    currentState = ScanState.PausingAtPoint;
                }
            }
        }

        void HandleRotating()
        {
            float currentYaw = lookEuler.Value.y;
            float newYaw = MoveTowardsAngle(currentYaw, targetYaw, yawSpeed * Time.deltaTime);

            SetRotation(lookEuler.Value.x, newYaw);

            // Đã đến góc mục tiêu
            if (Mathf.Abs(Mathf.DeltaAngle(newYaw, targetYaw)) < 1f)
            {
                SetRotation(lookEuler.Value.x, targetYaw);

                if (showDebugLogs)
                    Debug.Log($"[ScanAreaAtLKP] Đến điểm {currentStopPoint}/{scanStopPoints - 1} tại {targetYaw:F1}°");

                // Chuyển sang pause hoặc vertical scan
                if (scanVertically)
                {
                    isGoingUp = true;
                    targetPitch = topPitch;
                    currentState = ScanState.ScanningVertical;
                }
                else
                {
                    pauseTimer = 0f;
                    currentState = ScanState.PausingAtPoint;
                }
            }
        }

        void HandlePausing()
        {
            pauseTimer += Time.deltaTime;

            if (pauseTimer >= pauseDuration)
            {
                currentStopPoint++;

                // Đã quét đủ các điểm
                if (currentStopPoint >= scanStopPoints)
                {
                    if (showDebugLogs)
                        Debug.Log($"[ScanAreaAtLKP] Hoàn thành quét 180°, quay về trung tâm");

                    targetYaw = centerYaw;
                    currentState = ScanState.ReturningToCenter;
                }
                else
                {
                    // Tính góc cho điểm tiếp theo
                    float progress = (float)currentStopPoint / (scanStopPoints - 1);

                    if (startFromLeft)
                        targetYaw = NormalizeAngle(startScanYaw + (180f * progress));
                    else
                        targetYaw = NormalizeAngle(startScanYaw - (180f * progress));

                    currentState = ScanState.RotatingToNext;
                }
            }
        }

        void HandleVerticalScan()
        {
            float currentPitch = lookEuler.Value.x;
            float newPitch = Mathf.MoveTowards(currentPitch, targetPitch, pitchSpeed * Time.deltaTime);

            SetRotation(newPitch, lookEuler.Value.y);

            // Đã đến target pitch
            if (Mathf.Abs(newPitch - targetPitch) < 0.5f)
            {
                if (isGoingUp)
                {
                    // Đã nhìn lên, giờ nhìn xuống
                    isGoingUp = false;
                    targetPitch = bottomPitch;
                }
                else
                {
                    // Đã nhìn xuống, quay về giữa
                    targetPitch = 0f;

                    if (Mathf.Abs(newPitch) < 0.5f) // Đã về 0
                    {
                        pauseTimer = 0f;
                        currentState = ScanState.PausingAtPoint;
                    }
                }
            }
        }

        void HandleReturning()
        {
            float currentYaw = lookEuler.Value.y;
            float currentPitch = lookEuler.Value.x;

            float newYaw = MoveTowardsAngle(currentYaw, centerYaw, yawSpeed * Time.deltaTime);
            float newPitch = Mathf.MoveTowards(currentPitch, 0f, pitchSpeed * Time.deltaTime);

            SetRotation(newPitch, newYaw);

            // Đã về hướng LKP
            if (Mathf.Abs(Mathf.DeltaAngle(newYaw, centerYaw)) < 1f && Mathf.Abs(newPitch) < 0.5f)
            {
                SetRotation(0f, centerYaw);

                if (showDebugLogs)
                    Debug.Log($"[ScanAreaAtLKP] Hoàn thành, đang nhìn về hướng LKP");

                currentState = ScanState.Completed;
            }
        }

        void SetRotation(float pitch, float yaw)
        {
            lookEuler.Value = new Vector3(pitch, yaw, lookEuler.Value.z);
        }

        float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float delta = Mathf.DeltaAngle(current, target);
            if (Mathf.Abs(delta) <= maxDelta)
                return target;

            return current + Mathf.Sign(delta) * maxDelta;
        }

        float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            if (angle > 180f)
                angle -= 360f;
            else if (angle < -180f)
                angle += 360f;
            return angle;
        }

        public override void OnReset()
        {
            base.OnReset();
            currentState = ScanState.MovingToStartPosition;
            currentStopPoint = 0;
            pauseTimer = 0f;
        }
    }
}