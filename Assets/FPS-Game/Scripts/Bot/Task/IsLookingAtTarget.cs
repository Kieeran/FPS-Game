using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace CustomTask
{
    [TaskCategory("Custom")]
    public class IsLookingAtTarget : Conditional
    {
        [SerializeField] SharedTransform botCamera;
        [SerializeField] SharedTransform targetCamera;
        [SerializeField] SharedVector3 lookEuler;
        [SerializeField] float yawTolerance;
        [SerializeField] float pitchTolerance;

        public override TaskStatus OnUpdate()
        {
            if (botCamera.Value == null || targetCamera.Value == null)
                return TaskStatus.Failure;

            Vector3 dir = targetCamera.Value.position - botCamera.Value.position;
            if (dir.sqrMagnitude < 0.0001f)
                return TaskStatus.Failure;

            Vector3 norm = dir.normalized;

            float desiredYaw = Mathf.Atan2(norm.x, norm.z) * Mathf.Rad2Deg;
            float desiredPitch = -Mathf.Asin(norm.y) * Mathf.Rad2Deg;

            Vector3 currentEuler = botCamera.Value.eulerAngles;
            float currentYaw = currentEuler.y;
            float currentPitch = NormalizeAngle(currentEuler.x);

            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(currentYaw, desiredYaw));
            float pitchDelta = Mathf.Abs(Mathf.DeltaAngle(currentPitch, desiredPitch));

            bool isAligned =
                yawDelta <= yawTolerance &&
                pitchDelta <= pitchTolerance;
            return isAligned ? TaskStatus.Success : TaskStatus.Failure;
        }

        float NormalizeAngle(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }
    }
}