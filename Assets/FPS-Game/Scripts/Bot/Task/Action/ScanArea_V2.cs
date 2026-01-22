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
        [SerializeField] public SharedScanRange scanRange;
        [SerializeField] SharedVector3 lookEuler;

        [Header("Scan Settings")]
        [SerializeField] float sweepSpeed = 60f;
        [SerializeField] float pauseAtEdge = 0.5f;

        private float leftYaw;
        private float rightYaw;
        private float scanAngleRange;
        private bool isMovingToRight = true;
        private float pauseTimer;

        public override void OnStart()
        {
            base.OnStart();

            if (scanRange.Value == null) return;

            leftYaw = Quaternion.LookRotation(scanRange.Value.leftDir).eulerAngles.y;
            rightYaw = Quaternion.LookRotation(scanRange.Value.rightDir).eulerAngles.y;
            scanAngleRange = scanRange.Value.angleRange;

            float currentYaw = lookEuler.Value.y;

            // Tính khoảng cách góc THEO CHIỀU KIM ĐỒNG HỒ
            float distToLeft = CalculateClockwiseDistance(currentYaw, leftYaw);
            float distToRight = CalculateClockwiseDistance(currentYaw, rightYaw);

            // Chọn điểm gần hơn làm target đầu tiên
            isMovingToRight = distToRight < distToLeft;

            pauseTimer = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (scanRange.Value == null) return TaskStatus.Failure;

            if (pauseTimer > 0)
            {
                pauseTimer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            float currentYaw = lookEuler.Value.y;
            float targetYaw = isMovingToRight ? rightYaw : leftYaw;

            // Tính hướng xoay
            float distance = isMovingToRight
                ? CalculateClockwiseDistance(currentYaw, targetYaw)
                : CalculateCounterClockwiseDistance(currentYaw, targetYaw);

            float step = Mathf.Min(sweepSpeed * Time.deltaTime, distance);
            float newYaw = currentYaw + (isMovingToRight ? step : -step);

            // Normalize về [0, 360]
            newYaw = (newYaw % 360f + 360f) % 360f;

            lookEuler.Value = new Vector3(lookEuler.Value.x, newYaw, lookEuler.Value.z);

            // Kiểm tra đã chạm biên
            if (distance < 0.5f)
            {
                isMovingToRight = !isMovingToRight;
                pauseTimer = pauseAtEdge;
            }

            return TaskStatus.Running;
        }

        float CalculateClockwiseDistance(float from, float to)
        {
            float diff = to - from;
            if (diff < 0) diff += 360f;
            return diff;
        }

        float CalculateCounterClockwiseDistance(float from, float to)
        {
            float diff = from - to;
            if (diff < 0) diff += 360f;
            return diff;
        }

        public override void OnReset()
        {
            base.OnReset();
        }
    }
}