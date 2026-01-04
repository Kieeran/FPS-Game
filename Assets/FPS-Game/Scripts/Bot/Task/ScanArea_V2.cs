using System.Collections.Generic;
using AIBot;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class ScanArea_V2 : Action
    {
        [Header("References")]
        [SerializeField] SharedPointVisibilityData data; // Data chứa visibleIndices của điểm đang đứng
        [SerializeField] SharedPointVisibilityDataList visibilityMatrix; // Danh sách toàn bộ IPoint của Zone
        [SerializeField] SharedVector3 lookEuler; // Biến điều khiển góc xoay của Bot

        [Header("Scan Settings")]
        [SerializeField] float sweepSpeed = 60f; // Tốc độ lia tâm (độ/giây)

        // State Machine nội bộ
        enum State { MovingToStart, Sweeping, Completed }
        State currentState;

        float startYaw, endYaw;
        float currentProgress;
        List<int> pointsInThisScan; // Các điểm cần check tại vị trí này

        public override void OnStart()
        {
            base.OnStart();

            if (data.Value == null || data.Value.visibleIndices.Count == 0)
            {
                currentState = State.Completed;
                return;
            }

            pointsInThisScan = new List<int>(data.Value.visibleIndices);
            CalculateBoundaryAngles();

            currentProgress = 0;
            currentState = State.MovingToStart;
        }

        public override TaskStatus OnUpdate()
        {
            switch (currentState)
            {
                case State.MovingToStart:
                    HandleRotation(startYaw); // Quay nhanh tới điểm bắt đầu
                    if (IsAtAngle(startYaw)) currentState = State.Sweeping;
                    break;

                case State.Sweeping:
                    // Lia dần sang endYaw
                    currentProgress += sweepSpeed * Time.deltaTime / Mathf.Abs(Mathf.DeltaAngle(startYaw, endYaw));
                    float targetYaw = Mathf.LerpAngle(startYaw, endYaw, currentProgress);

                    HandleRotation(targetYaw);

                    if (currentProgress >= 1f) currentState = State.Completed;
                    break;

                case State.Completed:
                    return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        void CalculateBoundaryAngles()
        {
            float minAngle = float.MaxValue;
            float maxAngle = float.MinValue;

            Vector3 botPos = transform.position;
            Vector3 botForward = transform.forward;

            foreach (int idx in pointsInThisScan)
            {
                Vector3 targetPos = visibilityMatrix.Value[idx].position;
                Vector3 dir = (targetPos - botPos).normalized;
                float angle = Vector3.SignedAngle(botForward, dir, Vector3.up);

                if (angle < minAngle) minAngle = angle;
                if (angle > maxAngle) maxAngle = angle;
            }

            // Tính toán Yaw thế giới dựa trên Forward hiện tại của Bot
            float currentYaw = transform.eulerAngles.y;
            startYaw = NormalizeAngle(currentYaw + minAngle);
            endYaw = NormalizeAngle(currentYaw + maxAngle);
        }

        void HandleRotation(float targetYaw)
        {
            float currentYaw = lookEuler.Value.y;
            float newYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, sweepSpeed * Time.deltaTime);

            // Cập nhật biến SharedVariable để Controller thực thi xoay
            lookEuler.Value = new Vector3(lookEuler.Value.x, newYaw, lookEuler.Value.z);
        }

        bool IsAtAngle(float targetYaw)
        {
            return Mathf.Abs(Mathf.DeltaAngle(lookEuler.Value.y, targetYaw)) < 1f;
        }

        Vector3 GetLookDirection()
        {
            return Quaternion.Euler(lookEuler.Value.x, lookEuler.Value.y, 0) * Vector3.forward;
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }

        public override void OnReset()
        {
            currentState = State.MovingToStart;
            pointsInThisScan = null;
        }
    }
}