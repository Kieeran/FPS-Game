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
        [SerializeField] SharedVector3 lookEuler; // Biến điều khiển góc xoay của Bot

        [Header("Scan Settings")]
        [SerializeField] float sweepSpeed = 60f; // Tốc độ lia tâm (độ/giây)
        [SerializeField] float pauseAtEdge = 0.5f; // Thời gian khựng lại ở biên cho tự nhiên

        private float leftYaw;
        private float rightYaw;
        private bool isMovingToRight = true;
        private float pauseTimer;

        public override void OnStart()
        {
            base.OnStart();

            if (scanRange.Value == null) return;

            leftYaw = Quaternion.LookRotation(scanRange.Value.leftDir).eulerAngles.y;
            rightYaw = Quaternion.LookRotation(scanRange.Value.rightDir).eulerAngles.y;

            float currentYaw = lookEuler.Value.y;
            float distToLeft = Mathf.Abs(Mathf.DeltaAngle(currentYaw, leftYaw));
            float distToRight = Mathf.Abs(Mathf.DeltaAngle(currentYaw, rightYaw));

            // Nếu gần bên trái hơn, thì mục tiêu đầu tiên là bên phải (để bắt đầu quét ngay)
            // Nếu gần bên phải hơn, thì mục tiêu đầu tiên là bên trái
            isMovingToRight = distToLeft < distToRight;

            pauseTimer = 0;
        }

        public override TaskStatus OnUpdate()
        {
            if (scanRange.Value == null) return TaskStatus.Failure;

            // Xử lý nghỉ tại biên
            if (pauseTimer > 0)
            {
                pauseTimer -= Time.deltaTime;
                return TaskStatus.Running;
            }

            float currentYaw = lookEuler.Value.y;
            float currentTarget = isMovingToRight ? rightYaw : leftYaw;

            // Thực hiện xoay mượt
            float newYaw = Mathf.MoveTowardsAngle(currentYaw, currentTarget, sweepSpeed * Time.deltaTime);
            lookEuler.Value = new Vector3(lookEuler.Value.x, newYaw, lookEuler.Value.z);

            // Kiểm tra đã chạm biên chưa để đổi chiều
            if (Mathf.Abs(Mathf.DeltaAngle(newYaw, currentTarget)) < 0.5f)
            {
                isMovingToRight = !isMovingToRight;
                pauseTimer = pauseAtEdge;
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();
        }
    }
}