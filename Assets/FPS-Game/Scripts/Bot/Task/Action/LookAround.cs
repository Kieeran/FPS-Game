using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class LookAround : Action
    {
        [SerializeField] float topPitch;
        [SerializeField] float bottomPitch;
        [SerializeField] float leftYaw;
        [SerializeField] float rightYaw;
        [SerializeField] float speed;
        [SerializeField] float minWaitTime;
        [SerializeField] float maxWaitTime;
        [SerializeField] float angleVariation;      // Độ biến thiên ngẫu nhiên
        [SerializeField] SharedVector3 lookEuler;

        string mode = "Up";
        bool endTask = false;

        // Timer
        bool isWaiting = false;
        float waitTimer = 0f;
        float currentWaitDuration = 0f;
        string nextMode = "";

        // Random targets
        float targetPitch = 0f;
        float targetYaw = 0f;

        public override void OnStart()
        {
            base.OnStart();
            mode = "Up";
            endTask = false;
            isWaiting = false;
            waitTimer = 0f;
            lookEuler.Value = Vector3.zero;

            // Generate random first target
            GenerateRandomTarget("Up");
        }

        public override TaskStatus OnUpdate()
        {
            // Xử lý waiting
            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= currentWaitDuration)
                {
                    mode = nextMode;
                    isWaiting = false;
                    waitTimer = 0f;

                    // Generate new random target khi chuyển mode
                    GenerateRandomTarget(mode);
                }
                return TaskStatus.Running;
            }

            float pitch = lookEuler.Value.x;
            float yaw = lookEuler.Value.y;

            if (mode == "Up")
            {
                pitch = MoveTowards(pitch, targetPitch, speed);
                yaw = MoveTowards(yaw, targetYaw, speed);

                if (HasReachedTarget(pitch, yaw, targetPitch, targetYaw))
                {
                    Debug.Log($"Reached Up position: Pitch={pitch:F1}, Yaw={yaw:F1}");
                    StartWait("Normal");
                }
            }
            else if (mode == "Normal")
            {
                pitch = MoveTowards(pitch, 0f, speed);
                yaw = MoveTowards(yaw, 0f, speed);

                if (HasReachedTarget(pitch, yaw, 0f, 0f))
                {
                    Debug.Log("Back to normal");

                    if (endTask)
                    {
                        SetRotation(0f, 0f);
                        return TaskStatus.Success;
                    }

                    StartWait("Down");
                }
            }
            else if (mode == "Down")
            {
                pitch = MoveTowards(pitch, targetPitch, speed);
                yaw = MoveTowards(yaw, targetYaw, speed);

                if (HasReachedTarget(pitch, yaw, targetPitch, targetYaw))
                {
                    Debug.Log($"Reached Down position: Pitch={pitch:F1}, Yaw={yaw:F1}");
                    endTask = true;
                    StartWait("Normal");
                }
            }

            SetRotation(pitch, yaw);
            return TaskStatus.Running;
        }

        void GenerateRandomTarget(string targetMode)
        {
            if (targetMode == "Up")
            {
                // Nhìn lên + random yaw (trái hoặc phải)
                targetPitch = topPitch + Random.Range(-angleVariation, angleVariation);
                targetYaw = Random.value > 0.5f ?
                    leftYaw + Random.Range(-angleVariation, angleVariation) :
                    rightYaw + Random.Range(-angleVariation, angleVariation);
            }
            else if (targetMode == "Down")
            {
                // Nhìn xuống + random yaw (ngược lại với lần trước)
                targetPitch = bottomPitch + Random.Range(-angleVariation, angleVariation);

                // Nhìn hướng ngược lại với lần nhìn lên
                if (lookEuler.Value.y < 0) // Lần trước nhìn trái
                    targetYaw = rightYaw + Random.Range(-angleVariation, angleVariation);
                else // Lần trước nhìn phải
                    targetYaw = leftYaw + Random.Range(-angleVariation, angleVariation);
            }
            else if (targetMode == "Normal")
            {
                targetPitch = 0f;
                targetYaw = 0f;
            }

            // Clamp để không vượt quá giới hạn
            targetPitch = Mathf.Clamp(targetPitch, topPitch - angleVariation, bottomPitch + angleVariation);
            targetYaw = Mathf.Clamp(targetYaw, leftYaw - angleVariation, rightYaw + angleVariation);
        }

        void StartWait(string targetMode)
        {
            isWaiting = true;
            waitTimer = 0f;
            nextMode = targetMode;
            currentWaitDuration = Random.Range(minWaitTime, maxWaitTime);
        }

        bool HasReachedTarget(float currentPitch, float currentYaw, float targetPitch, float targetYaw)
        {
            float pitchDiff = Mathf.Abs(currentPitch - targetPitch);
            float yawDiff = Mathf.Abs(currentYaw - targetYaw);
            return pitchDiff <= 0.5f && yawDiff <= 0.5f;
        }

        float MoveTowards(float current, float target, float speed)
        {
            float step = speed * Time.deltaTime;
            return Mathf.MoveTowards(current, target, step);
        }

        void SetRotation(float pitch, float yaw)
        {
            var euler = new Vector3(pitch, yaw, lookEuler.Value.z);
            lookEuler.Value = euler;
        }

        public override void OnReset()
        {
            base.OnReset();
            mode = "Up";
            endTask = false;
            isWaiting = false;
            waitTimer = 0f;
            SetRotation(0f, 0f);
        }
    }
}